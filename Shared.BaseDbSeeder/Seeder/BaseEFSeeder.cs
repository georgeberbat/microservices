using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Shared.BaseDbSeeder.Seeder
{
    public abstract class BaseEFSeeder<TDbContext> : BaseSeeder<TDbContext>
        where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;

        protected BaseEFSeeder(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected override bool IsTransientException(Exception exception)
        {
            return exception is NpgsqlException { IsTransient: true };
        }

        protected override Task EnsureDeleted()
        {
            if (_dbContext is null)
            {
                throw new ArgumentNullException(nameof(_dbContext));
            }

            return _dbContext.Database.EnsureDeletedAsync();
        }

        protected override async Task MigrateAsync()
        {
            if (_dbContext is null)
            {
                throw new ArgumentNullException(nameof(_dbContext));
            }

            await _dbContext.Database.MigrateAsync();

            // Если миграция зарегистрировала новые типы в БД то они МОГУТ быть не видны в ближайших запросах.
            // Поэтому говорим БД сделать Flush.
            if (_dbContext.Database.GetDbConnection() is NpgsqlConnection npg)
            {
                npg.Open(); // Диспозить не должны.
                npg.ReloadTypes();
            }
        }
    }
}