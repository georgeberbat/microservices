using System;
using System.Threading;
using System.Threading.Tasks;
using Dex.Ef.Contracts.Entities;

namespace Shared.Dal
{
    public interface IWriteRepository<T, TK, out TReadRepository>
        where TReadRepository : IReadRepository<T, TK>
        where T : IEntity<TK>
        where TK : IComparable
    {
        TReadRepository Read { get; }

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> RemoveAsync(T entity, CancellationToken cancellationToken = default);
    }

    public interface IWriteRepository<T, in TK>
        where T : IEntity<TK>
        where TK : IComparable
    {
        IReadRepository<T, TK> Read { get; }

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> RemoveAsync(T entity, CancellationToken cancellationToken = default);
    }
}