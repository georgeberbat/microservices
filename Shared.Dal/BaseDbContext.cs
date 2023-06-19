using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dex.Cap.Common.Ef.Extensions;
using Dex.Ef.Contracts;
using GorodPay.Shared.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using Shared.Dal.Exceptions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Shared.Dal
{
    public abstract class BaseDbContext<T> : DbContext, IUnityOfWork, IWriteDbContext
        where T : DbContext
    {
        private readonly IModelStore _modelStore;
        public IQueryable<TEntity> Get<TEntity>() where TEntity : class => Set<TEntity>();

        public virtual bool IsReadOnly => ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking;

        protected BaseDbContext(DbContextOptions<T> options, ModelStore<T> modelStore)
            : base(options)
        {
            _modelStore = modelStore ?? throw new ArgumentNullException(nameof(modelStore));
            SavingChanges += (_, _) =>
            {
                if (IsReadOnly) throw new NotSupportedException("context is readonly");
            };
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                if (e.InnerException is PostgresException
                    {
                        IsTransient: false, SqlState: PostgresErrorCodes.UniqueViolation
                    } pge)
                {
                    throw new EntityAlreadyExistsException(pge.MessageText, e);
                }

                throw;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation,
            Func<CancellationToken, Task<bool>> verifySucceeded,
            TransactionScopeOption transactionScopeOption, IsolationLevel isolationLevel, uint timeoutInSeconds,
            CancellationToken cancellationToken)
        {
            await this.ExecuteInTransactionScopeAsync(operation, verifySucceeded, transactionScopeOption,
                isolationLevel, timeoutInSeconds, cancellationToken);
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation,
            Func<CancellationToken, Task<bool>> verifySucceeded, TransactionScopeOption transactionScopeOption,
            IsolationLevel isolationLevel,
            uint timeoutInSeconds, CancellationToken cancellationToken)
        {
            return await this.ExecuteInTransactionScopeAsync(operation, verifySucceeded, transactionScopeOption,
                isolationLevel, timeoutInSeconds,
                cancellationToken);
        }

        [SuppressMessage("Design", "CA1033:Методы интерфейса должны быть доступны для вызова дочерним типам")]
        async Task<TResult> IReadDbContext.ReadInTransactionScopeAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            Func<CancellationToken, Task<bool>> verifySucceeded,
            TransactionScopeOption transactionScopeOption,
            IsolationLevel isolationLevel,
            uint timeoutInSeconds,
            CancellationToken cancellationToken)
        {
            return await ExecuteInTransactionAsync(operation, verifySucceeded, transactionScopeOption, isolationLevel,
                timeoutInSeconds, cancellationToken);
        }

        // private
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            // register extensions
            modelBuilder.HasPostgresExtension("uuid-ossp");

            var sw = Stopwatch.StartNew();
            foreach (var modeType in _modelStore.GetModels())
            {
                modelBuilder.Entity(modeType);
            }

            Trace.WriteLine("OnModelCreating: " + sw.Elapsed);

            DateConverter(modelBuilder);
        }

        public async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            return connection;
        }

        private static void DateConverter(ModelBuilder builder)
        {
            var dateTimeConverter =
                new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            var dateTimeNullableConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

            builder.UseValueConverterForType(dateTimeConverter);
            builder.UseValueConverterForType(dateTimeNullableConverter);
        }
    }
}