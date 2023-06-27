using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dex.Cap.Outbox.Interfaces;
using Profile.Dal.Domain;
using Profile.Dal.Specifications;
using ProfileDomain;
using Shared.Dal;
using Shared.Dal.Exceptions;
using Shared.Password;

namespace Profile.Services
{
    public class UserService : IUserService
    {
        private readonly IWriteUserRepository _writeUserRepository;
        private readonly IMapper _mapper;
        private readonly IUnityOfWork _dbContext;
        private readonly IOutboxService<IUnityOfWork> _outboxService;
        private readonly IPasswordGenerator _passwordGenerator;

        public UserService(IWriteUserRepository writeUserRepository, IMapper mapper, IUnityOfWork dbContext,
            IOutboxService<IUnityOfWork> outboxService, IPasswordGenerator passwordGenerator)
        {
            _writeUserRepository = writeUserRepository;
            _mapper = mapper;
            _dbContext = dbContext;
            _outboxService = outboxService;
            _passwordGenerator = passwordGenerator;
        }

        public async Task<ProfileDomain.User> GetUser(Guid id, CancellationToken cancellationToken)
        {
            return await _writeUserRepository.Read.GetByIdAsync(id, cancellationToken);
        }

        public async Task<RegisterUserResponse> RegisterUser(IRegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var userId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            await _outboxService.ExecuteOperationAsync(correlationId,
                new { DbContext = _dbContext, Mapper = _mapper, Repository = _writeUserRepository },
                async (token, ctx) =>
                {
                    var userDb = (await ctx.State.Repository.Read
                            .FilterAsync(new ActiveUserByPhoneSpecification(request.Phone), token))
                        .FirstOrDefault();

                    if (userDb != null)
                    {
                        throw new EntityAlreadyExistsException($"User with number {request.Phone} already exists");
                    }

                    await ctx.State.Repository.AddAsync(new ProfileDomain.User
                    {
                        Id = userId,
                        Phone = request.Phone,
                        Password = _passwordGenerator.MakeHash(userId.ToString("N"), request.Password)
                    }, token);

                    await ctx.State.DbContext.SaveChangesAsync(token);
                }, cancellationToken);

            return new RegisterUserResponse { UserId = userId.ToString() };
        }

        public async Task DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _writeUserRepository.Read.GetByIdAsync(userId, cancellationToken);
            await _writeUserRepository.RemoveAsync(user, cancellationToken);
        }
    }
}