using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Shared.Dal
{
    public interface IUnityOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            Func<CancellationToken, Task<bool>> verifySucceeded,
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            uint timeoutInSeconds = 60,
            CancellationToken cancellationToken = default);

        Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            Func<CancellationToken, Task<bool>> verifySucceeded,
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            uint timeoutInSeconds = 60,
            CancellationToken cancellationToken = default);
    }
}