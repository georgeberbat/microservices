using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Profile.Services;

namespace Profile;

public class ProfileGrpcService : ProfileGrpc.ProfileGrpcBase
{
    private readonly IUserService _userService;

    public ProfileGrpcService(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public override async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request,
        ServerCallContext context)
    {
        return await _userService.RegisterUser(request, context.CancellationToken);
    }

    public override async Task<Empty> DeleteMyself(GuidKey request, ServerCallContext context)
    {
        await _userService.DeleteUser(Guid.Parse(request.Id), context.CancellationToken);
        return new Empty();
    }
}