using Dex.Ef.Contracts.Entities;
using Shared.Dal;
using Shared.Dal.Specifications;

namespace Tariff.Services;

public abstract class BaseEntityUnitService<T, TU> : IBaseService<T, TU> where T : IEntity<Guid>, IDeletable
    where TU : IEntity<Guid>
{
    private readonly IWriteRepository<T, Guid, IReadRepository<T, Guid>> _writeRepository;
    private readonly IWriteRepository<TU, Guid, IReadRepository<TU, Guid>> _writeUnitRepository;
    private readonly IUnityOfWork _dbContext;


    protected BaseEntityUnitService(IWriteRepository<T, Guid, IReadRepository<T, Guid>> writeRepository,
        IUnityOfWork dbContext, IWriteRepository<TU, Guid, IReadRepository<TU, Guid>> writeUnitRepository)
    {
        _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _writeUnitRepository = writeUnitRepository;
    }

    public async Task<IEnumerable<T>> Get(Guid userId, CancellationToken cancellationToken)
    {
        return await _writeRepository.Read.FilterAsync(new UndeletedSpecification<T>(),
            cancellationToken);
    }

    public abstract Task<IEnumerable<T>> GetAvailableToMe(Guid userId, CancellationToken cancellationToken);

    public async Task<Guid> Create(Guid userId, T entity, CancellationToken cancellationToken)
    {
        await _writeRepository.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task Update(Guid userId, T entity, CancellationToken cancellationToken)
    {
        var exists = await _writeRepository.Read.GetByIdAsync(entity.Id, cancellationToken);
        await _writeRepository.UpdateAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var entity = await _writeRepository.Read.GetByIdAsync(id, cancellationToken);
        await _writeRepository.RemoveAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public abstract Task GrantPermission(Guid masterUserId, Guid slaveUserId, Guid resourceId, CancellationToken cancellationToken);
    public abstract Task<IEnumerable<Guid>> CreateEntityUnits(Guid userId, IEnumerable<TU> units, CancellationToken cancellationToken);
    public abstract Task UpdateEntityUnits(Guid userId, IEnumerable<TU> units, CancellationToken cancellationToken);
    public abstract Task DeleteEntityUnits(Guid userId, IEnumerable<Guid> ids, CancellationToken cancellationToken);
}