using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Dal.Extensions;

namespace Shared.Dal.Seeder
{
    // ReSharper disable once InconsistentNaming
    public abstract class BaseEFSeeder<TDbContext> : BaseSeeder<TDbContext>
        where TDbContext : DbContext
    {
        protected BaseEFSeeder(int delaySeconds = 2) : base(delaySeconds)
        {
        }

        protected void AddDbContext(string connectionString)
        {
            Services.AddDbContext<TDbContext>(connectionString);
        }

        protected override bool IsTransientException(Exception exception)
        {
            return exception is NpgsqlException {IsTransient: true};
        }

        protected override Task EnsureDeleted(TDbContext dbContext)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            return dbContext.Database.EnsureDeletedAsync();
        }

        protected override async Task MigrateAsync(TDbContext dbContext)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            await dbContext.Database.MigrateAsync();

            // Если миграция зарегистрировала новые типы в БД то они МОГУТ быть не видны в ближайших запросах.
            // Поэтому говорим БД сделать Flush.
            if (dbContext.Database.GetDbConnection() is NpgsqlConnection npg)
            {
                if (npg.State != ConnectionState.Open)
                {
                    await npg.OpenAsync(); // Диспозить не должны.
                }

                npg.ReloadTypes();
            }
        }
    }
}