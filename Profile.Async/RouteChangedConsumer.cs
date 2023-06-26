using MassTransit;
using Profile.Dal.Domain;
using ProfileDomain;
using ProfileDomain.Commands;
using Shared.Dal;

namespace Profile.Async;

public class RouteChangedConsumer : IConsumer<RouteChangedCommand>
{
    private readonly IWriteNotificationRepository _repository;
    private readonly IUnityOfWork _unityOfWork;

    public RouteChangedConsumer(IWriteNotificationRepository repository, IUnityOfWork unityOfWork)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unityOfWork = unityOfWork ?? throw new ArgumentNullException(nameof(unityOfWork));
    }

    public async Task Consume(ConsumeContext<RouteChangedCommand> context)
    {
        var command = context.Message;
        var cancellationToken = context.CancellationToken;
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Text = command.Text,
            Title = command.Title,
            Viewed = false
        };

        await _repository.AddAsync(notification, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
    }
}