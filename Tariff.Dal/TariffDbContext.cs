using System.Runtime.CompilerServices;
using Dex.Cap.Outbox.Ef;
using GorodPay.Shared.Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Dal;
using Tariff.Models;

namespace Tariff.Dal
{

    public class TariffDbContext : BaseDbContext<TariffDbContext>
    {
        public DbSet<Models.Tariff> Tariff => Set<Models.Tariff>();
        public DbSet<TariffUnit> TariffUnit => Set<TariffUnit>();
        public DbSet<Route> Route => Set<Route>();
        public DbSet<RouteUnit> RouteUnit => Set<RouteUnit>();

#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void RegisterEnums()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PermissionMode>();
        }
        
        public TariffDbContext(DbContextOptions<TariffDbContext> options, ModelStore<TariffDbContext> modelStore)
            : base(options, modelStore)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.HasPostgresEnum<PermissionMode>();
            base.OnModelCreating(modelBuilder);
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
            modelBuilder.OutboxModelCreating();
        }
    }
}