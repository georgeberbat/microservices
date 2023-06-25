using Microsoft.Extensions.Internal;
using Shared.Dal;
using Tariff.Dal.Domain;
using Tariff.Dal.Domain.Tariff;

namespace Tariff.Dal.Repositories
{
    internal class WriteTariffRepository : GenericWriteRepository<Models.Tariff, Guid, IReadTariffRepository>,
        IWriteTariffRepository
    {
        private readonly ISystemClock _systemClock;

        public WriteTariffRepository(IWriteDbContext writeDbContext, ISystemClock systemClock)
            : base(writeDbContext, new ReadTariffRepository(writeDbContext))
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }
    }
}