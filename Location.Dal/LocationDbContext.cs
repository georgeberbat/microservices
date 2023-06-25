using System.Runtime.CompilerServices;
using Dex.Cap.Outbox.Ef;
using GorodPay.Shared.Dal.Extensions;
using Location.Dal.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Shared.Dal;

namespace Location.Dal
{

    public class LocationDbContext : BaseDbContext<LocationDbContext>
    {
        public DbSet<Models.Location> Locations => Set<Models.Location>();

#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void RegisterEnums()
        {
        }
        
        public LocationDbContext(DbContextOptions<LocationDbContext> options, ModelStore<LocationDbContext> modelStore)
            : base(options, modelStore)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.ApplyConfiguration(new LocationConfiguration());

            base.OnModelCreating(modelBuilder);
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
            modelBuilder.OutboxModelCreating();
        }
    }
}