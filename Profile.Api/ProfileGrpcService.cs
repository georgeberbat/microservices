using System;
using System.Linq;
using System.Threading.Tasks;
using Dex.Specifications;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Profile.Dal.Domain;
using Profile.Services;
using Shared.Dal;
using Shared.Helpers;

namespace Profile;

public class ProfileGrpcService : ProfileGrpc.ProfileGrpcBase
{
    private readonly IUserService _userService;
    private readonly IWriteNotificationRepository _notificationRepository;
    private readonly IUnityOfWork _unityOfWork;
    

    public ProfileGrpcService(IUserService userService, IWriteNotificationRepository notificationRepository, IUnityOfWork unityOfWork)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _notificationRepository =
            notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _unityOfWork = unityOfWork ?? throw new ArgumentNullException(nameof(unityOfWork));
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

    public override async Task<GetNotificationsResponse> GetNotifications(GetNotificationsRequest request,
        ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var notifications = await _notificationRepository.Read.FilterAsync(
            new Specification<ProfileDomain.Notification>(x => x.UserId == userId && x.Viewed == request.Viewed),
            context.CancellationToken);

        var response = new GetNotificationsResponse
        {
            Notifications = { notifications.Select(x => new Notification
            {
                Id = x.Id.ToString(),
                UserId = x.UserId.ToString(),
                CreatedUtc = Timestamp.FromDateTime(x.CreatedUtc.SetUtcKind()),
                Text = x.Text,
                Title = x.Title,
                Viewed = x.Viewed
            }) }
        };
        
        return response;
    }

    public override async Task<Empty> MarkAsRead(MarkAsReadRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var ids = request.Ids.Select(Guid.Parse).ToArray();
        
        var notifications = await _notificationRepository.Read.FilterAsync(
            new Specification<ProfileDomain.Notification>(x => x.UserId == userId && ids.Contains(x.Id)),
            context.CancellationToken);

        foreach (var notification in notifications)
        {
            notification.Viewed = true;
            await _notificationRepository.UpdateAsync(notification);
        }

        await _unityOfWork.SaveChangesAsync(context.CancellationToken);
        return new Empty();
    }
}