using AutoMapper;
using Dex.Specifications;
using Location.Dal;
using Shared.Dal;
using Shared.Dal.Specifications;
using Shared.Exceptions;
using Tariff.Dal.Domain.Tariff;
using Tariff.Models;

namespace Tariff.Services;

public class TariffService : ITariffService
{
    private readonly IWriteTariffRepository _writeRepository;
    private readonly IWriteTariffUnitRepository _writeTariffUnitRepository;
    private readonly ReadLocationDbContext _readLocationDbContext;
    private readonly IUnityOfWork _dbContext;
    private readonly IMapper _mapper;


    public TariffService(IWriteTariffRepository writeRepository, IUnityOfWork dbContext, IMapper mapper,
        ReadLocationDbContext readLocationDbContext, IWriteTariffUnitRepository writeTariffUnitRepository)
    {
        _writeRepository = writeRepository;
        _dbContext = dbContext;
        _mapper = mapper;
        _readLocationDbContext = readLocationDbContext;
        _writeTariffUnitRepository = writeTariffUnitRepository;
    }

    public async Task<IEnumerable<Models.Tariff>> Get(Guid userId, CancellationToken cancellationToken)
    {
        return await _writeRepository.Read.FilterAsync(new UndeletedSpecification<Models.Tariff>(),
            cancellationToken);
    }

    public async Task<Guid> Create(Guid userId, Models.Tariff entity, CancellationToken cancellationToken)
    {
        var tariff = await _writeRepository.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return tariff.Id;
    }

    public async Task Update(Guid userId, Models.Tariff entity, CancellationToken cancellationToken)
    {
        var exist = await _writeRepository.Read.GetByIdAsync(entity.Id, cancellationToken);
        if (userId != exist.UserId && entity.UserId != userId)
        {
            throw new NotAllowedException();
        }

        _mapper.Map(entity, exist);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var exist = await _writeRepository.Read.GetByIdAsync(id, cancellationToken);

        if (userId != exist.UserId)
        {
            throw new NotAllowedException();
        }

        await _writeRepository.RemoveAsync(exist, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> CreateEntityUnits(Guid userId, IEnumerable<TariffUnit> units,
        CancellationToken cancellationToken)
    {
        units = units as TariffUnit[] ?? units.ToArray();
        if (!units.Any()) return ArraySegment<Guid>.Empty;

        var tariffId = units.First().TariffId;
        if (units.Any(x => x.TariffId != tariffId))
        {
            throw new BadRequestException(units.Where(x => x.TariffId != tariffId)
                .Select(x => (nameof(units), (object)x)).ToArray());
        }

        var tariff = await _writeRepository.Read.GetByIdAsync(tariffId, cancellationToken);
        if (tariff.UserId != userId)
        {
            throw new NotAllowedException();
        }

        await _readLocationDbContext.CheckExistence(units.Select(x => x.LocationId), cancellationToken);

        foreach (var unit in units)
        {
            unit.Id = Guid.NewGuid();
            await _writeTariffUnitRepository.AddAsync(unit, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return units.Select(x => x.Id);
    }

    public async Task UpdateEntityUnits(Guid userId, IEnumerable<TariffUnit> units, CancellationToken cancellationToken)
    {
        units = units as TariffUnit[] ?? units.ToArray();
        if (!units.Any()) return;

        var tariffId = units.First().TariffId;

        var tariff = await _writeRepository.Read.GetByIdAsync(tariffId, cancellationToken);
        if (tariff.UserId != userId)
        {
            throw new NotAllowedException();
        }

        await _readLocationDbContext.CheckExistence(units.Select(x => x.LocationId), cancellationToken);

        foreach (var unit in units)
        {
            var unitExist = tariff.TariffUnits!.FirstOrDefault(x => x.Id == unit.Id);
            if (unitExist != null)
            {
                _mapper.Map(unit, unitExist);
            }
            else
            {
                unit.Id = Guid.NewGuid();
                await _writeTariffUnitRepository.AddAsync(unit, cancellationToken);
            }
        }

        await _writeRepository.UpdateAsync(tariff, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteEntityUnits(Guid userId, IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        ids = ids as Guid[] ?? ids.ToArray();
        if (!ids.Any()) return;

        var tariffUnitId = ids.First();

        var tariff = await _writeRepository.Read.GetBySpecAsync(
            new Specification<Models.Tariff>(x =>
                x.TariffUnits != null && x.TariffUnits.Any(y => y.Id == tariffUnitId)), cancellationToken);

        if (tariff.UserId != userId)
        {
            throw new NotAllowedException();
        }

        var tariffUnits =
            await _writeTariffUnitRepository.Read.FilterAsync(new Specification<TariffUnit>(x => ids.Contains(x.Id)),
                cancellationToken);

        foreach (var tariffUnit in tariffUnits)
        {
            await _writeTariffUnitRepository.RemoveAsync(tariffUnit, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}