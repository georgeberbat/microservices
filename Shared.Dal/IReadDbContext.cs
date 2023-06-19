using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Shared.Dal
{
    public interface IReadDbContext
    {
        IQueryable<T> Get<T>() where T : class;

        Task<TResult> ReadInTransactionScopeAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            Func<CancellationToken, Task<bool>> verifySucceeded,
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Suppress,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            uint timeoutInSeconds = 60,
            CancellationToken cancellationToken = default);
    }
}