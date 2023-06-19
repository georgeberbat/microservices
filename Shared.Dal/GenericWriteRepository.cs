using System;
using System.Threading;
using System.Threading.Tasks;
using Dex.Ef.Contracts.Entities;

namespace Shared.Dal
{
    public class GenericWriteRepository<T, TK, TReadRepository> : IWriteRepository<T, TK, TReadRepository>
        where TReadRepository : IReadRepository<T, TK>
        where T : IEntity<TK>
        where TK : IComparable
    {
        public TReadRepository Read { get; }
        private IWriteDbContext WriteDbContext { get; }

        public GenericWriteRepository(IWriteDbContext writeDbContext, TReadRepository readRepository)
        {
            Read = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
            WriteDbContext = writeDbContext ?? throw new ArgumentNullException(nameof(writeDbContext));
        }

        public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return Task.FromResult((T) WriteDbContext.Add(entity).Entity);
        }

        public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return Task.FromResult((T) WriteDbContext.Update(entity).Entity);
        }

        public virtual Task<T> RemoveAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return Task.FromResult((T) WriteDbContext.Remove(entity).Entity);
        }
    }

    public class GenericWriteRepository<T, TK> : GenericWriteRepository<T, TK, IReadRepository<T, TK>>, IWriteRepository<T, TK>
        where T : IEntity<TK>
        where TK : IComparable
    {
        public GenericWriteRepository(IWriteDbContext writeDbContext, IReadRepository<T, TK> readRepository)
            : base(writeDbContext, readRepository)
        {
        }
    }
}