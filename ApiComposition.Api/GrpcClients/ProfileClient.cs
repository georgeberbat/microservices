using ApiComposition.Models;

namespace ApiComposition.Api.GrpcClients;

public class ProfileClient
{
    private readonly ProfileGrpc.ProfileGrpcClient _client;

    public ProfileClient(ProfileGrpc.ProfileGrpcClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<RegisterUserResponse> RegisterUser(IRegistrationRequest request)
    {
        return await _client.RegisterUserAsync(new RegisterUserRequest
        {
            Phone = request.Phone,
            Password = request.Password
        });
    }
}