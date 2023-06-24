using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dex.Ef.Contracts.Entities;
using Dex.Specifications;

namespace Shared.Dal
{
    public interface IReadRepository<T, in TK>
        where T : IEntity<TK>
        where TK : IComparable
    {
        Task<T> GetByIdAsync(TK id, CancellationToken cancellation);
        Task<T> GetBySpecAsync(Specification<T> specification, CancellationToken cancellation);

        Task<T[]> FilterAsync(Specification<T> specification, CancellationToken cancellation);

        Task<IEnumerable<T>> SearchBySubstring(string substring, Func<T, string> propertyNameGetter,
            CancellationToken token);
    }
}