using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dex.Ef.Contracts.Entities;
using Dex.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Dal.Exceptions;

namespace Shared.Dal
{
    public class GenericReadRepository<T, TK> : IReadRepository<T, TK>
        where T : class, IEntity<TK> where TK : IComparable
    {
        private IReadDbContext DbContext { get; }
        protected virtual IQueryable<T> BaseQuery => DbContext.Get<T>();

        public GenericReadRepository(IReadDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<T> GetByIdAsync(TK id, CancellationToken cancellation)
        {
            var result = await BaseQuery.FirstOrDefaultAsync(x => Equals(x.Id, id), cancellation);
            if (result == null)
                throw new EntityNotFoundException(id.ToString()!, typeof(T).Name);

            return result;
        }

        public async Task<T> GetBySpecAsync(Specification<T> specification, CancellationToken cancellation)
        {
            var result = await BaseQuery.FirstOrDefaultAsync(specification, cancellation);
            if (result == null)
                throw new EntityNotFoundException<T>();

            return result;
        }

        public Task<T[]> FilterAsync(Specification<T> specification, CancellationToken cancellation)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return BaseQuery.Where(specification).ToArrayAsync(cancellation);
        }
    }
}