using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Domain
{
    public interface IWriteNotificationRepository : IWriteRepository<Notification, Guid, IReadNotificationRepository>
    {
    }
}