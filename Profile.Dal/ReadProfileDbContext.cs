using System;
using System.Runtime.CompilerServices;
using GorodPay.Shared.Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Profile.Dal.EntityConfigurations;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal
{

    public class ReadProfileDbContext : BaseDbContext<ReadProfileDbContext>, IReadProfileDbContext
    {
        public DbSet<User> Users => Set<User>();

#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void RegisterEnums()
        {
        }
        
        public override bool IsReadOnly => true;
        
        public ReadProfileDbContext(DbContextOptions<ReadProfileDbContext> options, ModelStore<ReadProfileDbContext> modelStore)
            : base(options, modelStore)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());

            base.OnModelCreating(modelBuilder);
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        }
    }
}