using System;
using Microsoft.Extensions.Internal;
using Profile.Dal.Domain;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Repositories
{
    internal class WriteNotificationRepository : GenericWriteRepository<Notification, Guid, IReadNotificationRepository>, IWriteNotificationRepository
    {
        private readonly ISystemClock _systemClock;

        public WriteNotificationRepository(IWriteDbContext writeDbContext, ISystemClock systemClock)
            : base(writeDbContext, new ReadNotificationRepository(writeDbContext))
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }
    }
}