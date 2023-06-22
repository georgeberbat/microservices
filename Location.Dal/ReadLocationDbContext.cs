using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Shared.Dal;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);
        }
    }
}