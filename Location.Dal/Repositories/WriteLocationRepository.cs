using Location.Dal.Domain;
using Microsoft.Extensions.Internal;
using Shared.Dal;

namespace Location.Dal.Repositories
{
    internal class WriteLocationRepository : GenericWriteRepository<Models.Location, Guid, IReadLocationRepository>,
        IWriteLocationRepository
    {
        private readonly ISystemClock _systemClock;

        public WriteLocationRepository(IWriteDbContext writeDbContext, ISystemClock systemClock)
            : base(writeDbContext, new ReadLocationRepository(writeDbContext))
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }
        
        
    }
}