using Dex.Ef.Contracts.Entities;

namespace Tariff.Services;

public interface IBaseService<T, in TU>  where T : IEntity<Guid> where TU : IEntity<Guid>
{
    Task<IEnumerable<T>> Get(Guid userId, CancellationToken cancellationToken);
    Task<Guid> Create(Guid userId, T entity, CancellationToken cancellationToken);
    Task Update(Guid userId, T entity, CancellationToken cancellationToken);
    Task Delete(Guid userId, Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Guid>> CreateEntityUnits(Guid userId, IEnumerable<TU> units, CancellationToken cancellationToken);
    Task UpdateEntityUnits(Guid userId, IEnumerable<TU> units, CancellationToken cancellationToken);
    Task DeleteEntityUnits(Guid userId, IEnumerable<Guid> ids, CancellationToken cancellationToken);
}