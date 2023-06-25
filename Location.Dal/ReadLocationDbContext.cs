using System.Runtime.CompilerServices;
using GorodPay.Shared.Dal.Extensions;
using Location.Dal.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Shared.Dal;
using Shared.Exceptions;

namespace Location.Dal
{
    public class ReadLocationDbContext : BaseDbContext<ReadLocationDbContext>, IReadLocationDbContext
    {
        public DbSet<Models.Location> Locations => Set<Models.Location>();

#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void RegisterEnums()
        {
        }

        public override bool IsReadOnly => true;

        public ReadLocationDbContext(DbContextOptions<ReadLocationDbContext> options,
            ModelStore<ReadLocationDbContext> modelStore)
            : base(options, modelStore)
        {
        }
        
        public Task CheckExistence(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            var exists = Locations.Where(x => ids.Contains(x.Id)).Select(x => x.Id);

            if (ids.Count() < exists.Count())
            {
                throw new BadRequestException(); 
            }

            return Task.FromResult(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.ApplyConfiguration(new LocationConfiguration());

            base.OnModelCreating(modelBuilder);
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        }
    }
}