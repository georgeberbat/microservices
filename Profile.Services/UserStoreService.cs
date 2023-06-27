using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Profile.Dal.Domain;
using Profile.Dal.Specifications;
using Shared.Dal.Exceptions;
using Shared.Password;

namespace Profile.Services;

public class UserStoreService : UserStore.UserStoreBase
{
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IReadUserRepository _readUserRepository;
    private readonly IMapper _mapper;

    public UserStoreService(IPasswordGenerator passwordGenerator, IReadUserRepository readUserRepository, IMapper mapper)
    {
        _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
        _readUserRepository = readUserRepository ?? throw new ArgumentNullException(nameof(readUserRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override async Task<AuthResult> ValidateCredentials(Credential request, ServerCallContext context)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (context == null) throw new ArgumentNullException(nameof(context));

        try
        {
            var user = await _readUserRepository
                .GetBySpecAsync(new ActiveUserByPhoneSpecification(request.Username), CancellationToken.None)
                .ConfigureAwait(false);
            var expected = _passwordGenerator.MakeHash(user.Id.ToString("N"), request.Password);
            if (user.Password == expected)
            {
                return new AuthResult { Success = true, Sub = user.Id.ToString() };
            }
        }
        catch (EntityNotFoundException)
        {
            // ignored
        }

        return new AuthResult { Message = "invalid credentials", ErrorCode = AuthErrorCode.NotFound };
    }
    
    public override async Task<User> FindBySubjectId(Subject request, ServerCallContext context)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (!Guid.TryParse((ReadOnlySpan<char>) request.Sub, out var sub))
        {
            context.Status = new Status(StatusCode.InvalidArgument, "sub is invalid:" + request.Sub);
            return User.Empty;
        }

        try
        {
            var user = await _readUserRepository
                .GetByIdAsync(sub, CancellationToken.None)
                .ConfigureAwait(false);
            
            var foundUser = _mapper.Map<ProfileDomain.User, User>(user);
            foundUser.Claims.AddRange(FillClaims(user, request.Claims));
            return foundUser;
        }
        catch (EntityNotFoundException)
        {
            // ignored
        }

        context.Status = new Status(StatusCode.NotFound, "user not found by sub:" + request.Sub);
        return User.Empty;
    }
    
    private static IEnumerable<SimpleClaim> FillClaims(ProfileDomain.User userDb, IEnumerable<SimpleClaim> requestClaims)
    {
        foreach (var claim in requestClaims)
        {
            if (claim.Type == "name")
            {
                var value = $"{userDb?.LastName} {userDb?.FirstName}";
                yield return new SimpleClaim(claim) {Value = value};
            }
        }
    }
}