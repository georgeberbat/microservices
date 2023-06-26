using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Domain
{
    public interface IReadNotificationRepository : IReadRepository<Notification, Guid>
    {
    }
}