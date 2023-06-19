using System;
using Microsoft.Extensions.Internal;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Repositories
{
    internal class WriteUserRepository : GenericWriteRepository<User, Guid, IReadUserRepository>, IWriteUserRepository
    {
        private readonly ISystemClock _systemClock;

        public WriteUserRepository(IWriteDbContext writeDbContext, ISystemClock systemClock)
            : base(writeDbContext, new ReadUserRepository(writeDbContext))
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }
    }
}