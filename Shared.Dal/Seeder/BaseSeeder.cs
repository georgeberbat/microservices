using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Serilog;

namespace Shared.Dal.Seeder
{
    /// <summary>
    /// Сеятель данных, начальное наполнение.
    /// </summary>
    /// <typeparam name="TDbContext">Контекст данных.</typeparam>
    public abstract class BaseSeeder<TDbContext> where TDbContext : notnull
    {
        private readonly TimeSpan _delay;
        public ServiceProvider ServiceProvider { get; private set; }

        protected BaseSeeder(int delaySeconds = 2)
        {
            if (delaySeconds <= 0) throw new ArgumentOutOfRangeException(nameof(delaySeconds));

            _delay = TimeSpan.FromSeconds(Math.Max(1, delaySeconds));
            Services.AddSingleton<ISystemClock, SystemClock>();
        }

        protected IServiceCollection Services { get; } = new ServiceCollection();

        public async Task RunAsync(bool ensureDeleted = false, bool inMemory = false)
        {
            Log.Debug("Seeding database [{Database}]", typeof(TDbContext).FullName);

            ServiceProvider = Services.BuildServiceProvider();
            

            var repeatCount = 10;
            Exception? lastException = null;

            do
            {
                Log.Debug("Try migrate database [{Database}]. Repeat: {RepeatCount}", typeof(TDbContext).FullName, repeatCount);

                try
                {
                    using (var scope = ServiceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                        if (ensureDeleted)
                        {
                            await EnsureDeleted(dbContext);
                        }

                        Log.Debug("Экзепляр контекста: {DBContext}", dbContext.GetType().FullName);
                        if (!inMemory)
                            await MigrateAsync(dbContext);
                        await EnsureSeedData(dbContext);
                    }

                    Log.Debug("Migrate database [{Database}] completed", typeof(TDbContext).FullName);
                    break;
                }
                catch (Exception ex) when (IsTransientException(ex))
                {
                    lastException = ex;
                    Thread.Sleep(_delay);
                }
            } while (--repeatCount > 0);

            if (lastException != null)
            {
                throw new InvalidOperationException($"Migrate database [{typeof(TDbContext).FullName}] failed.", lastException);
            }
        }

        protected abstract bool IsTransientException(Exception exception);

        protected abstract Task EnsureDeleted(TDbContext dbContext);

        protected abstract Task MigrateAsync(TDbContext dbContext);

        protected abstract Task EnsureSeedData(TDbContext dbContext);
    }
}