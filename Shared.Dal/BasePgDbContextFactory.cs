using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Npgsql;

namespace Shared.Dal
{
    public abstract class BasePgDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        public TDbContext CreateDbContext(string[] args)
        {
            var sc = CreateServiceCollection();

            var builder = new DbContextOptionsBuilder<TDbContext>();
            builder.UseNpgsql();
            builder.UseApplicationServiceProvider(sc.BuildServiceProvider());

            return CreateInstance(builder.Options);
        }

        protected virtual ServiceCollection CreateServiceCollection()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<ISystemClock, SystemClock>();
            sc.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
            return sc;
        }

        protected abstract TDbContext CreateInstance(DbContextOptions<TDbContext> options);
    }
}