using ApiComposition.Models;
using Shared.Interfaces;

namespace ApiComposition.Api.GrpcClients;

public class ProfileClient
{
    private readonly ProfileGrpc.ProfileGrpcClient _client;
    private readonly IUserIdHttpContextService _userId;

    public ProfileClient(ProfileGrpc.ProfileGrpcClient client, IUserIdHttpContextService userId)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _userId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    public async Task<RegisterUserResponse> RegisterUser(IRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        return await _client.RegisterUserAsync(new RegisterUserRequest
        {
            Phone = request.Phone,
            Password = request.Password
        }, cancellationToken: cancellationToken);
    }

    public async Task DeleteMyself(CancellationToken cancellationToken)
    {
        await _client.DeleteMyselfAsync(new GuidKey
        {
            Id = _userId.UserId.ToString()
        }, cancellationToken: cancellationToken);
    }

    public async Task<GetNotificationsResponse> GetNotifications(GetNotificationsRequest request,
        CancellationToken cancellationToken)
    {
        return await _client.GetNotificationsAsync(request, cancellationToken: cancellationToken);
    }
    
    public async Task MarkAsRead(MarkAsReadRequest request,
        CancellationToken cancellationToken)
    {
        await _client.MarkAsReadAsync(request, cancellationToken: cancellationToken);
    }
}