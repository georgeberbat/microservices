using System.Runtime.CompilerServices;
using GorodPay.Shared.Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Dal;
using Tariff.Models;

namespace Tariff.Dal
{
    public class ReadTariffDbContext : BaseDbContext<ReadTariffDbContext>, IReadTariffDbContext
    {
        public DbSet<Models.Tariff> Tariff => Set<Models.Tariff>();
        public DbSet<Route> Route => Set<Route>();

#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void RegisterEnums()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PermissionMode>();
        }

        public override bool IsReadOnly => true;

        public ReadTariffDbContext(DbContextOptions<ReadTariffDbContext> options,
            ModelStore<ReadTariffDbContext> modelStore)
            : base(options, modelStore)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        }
    }
}