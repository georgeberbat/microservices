using System;
using Profile.Dal.Domain;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Repositories
{
    internal class ReadNotificationRepository : GenericReadRepository<Notification, Guid>, IReadNotificationRepository
    {
        public ReadNotificationRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }
    }
}