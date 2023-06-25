using AutoMapper;
using Dex.Specifications;
using Shared.Dal;
using Shared.Exceptions;
using Tariff.Dal.Domain.Tariff;
using Tariff.Models;

namespace Tariff.Services;

public class RouteService : IRouteService
{
    private readonly IWriteRouteRepository _writeRepository;
    private readonly IWriteRouteUnitRepository _writeRouteUnitRepository;
    
    private readonly IReadTariffRepository _tariffRepository;
    
    private readonly IUnityOfWork _dbContext;
    private readonly IMapper _mapper;


    public RouteService(IWriteRouteRepository writeRepository, IUnityOfWork dbContext, IMapper mapper,
        IWriteRouteUnitRepository writeRouteUnitRepository, IReadTariffRepository tariffRepository)
    {
        _writeRepository = writeRepository;
        _dbContext = dbContext;
        _mapper = mapper;
        _writeRouteUnitRepository = writeRouteUnitRepository;
        _tariffRepository = tariffRepository;
    }

    public async Task<IEnumerable<Route>> Get(Guid userId, CancellationToken cancellationToken)
    {
        return await _writeRepository.Read.FilterAsync(new Specification<Route>(_ => true),
            cancellationToken);
    }

    public async Task<Guid> Create(Guid userId, Route entity, CancellationToken cancellationToken)
    {
        var route = await _writeRepository.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return route.Id;
    }

    public async Task Update(Guid userId, Route entity, CancellationToken cancellationToken)
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

    public async Task<IEnumerable<Guid>> CreateEntityUnits(Guid userId, IEnumerable<RouteUnit> units,
        CancellationToken cancellationToken)
    {
        units = units as RouteUnit[] ?? units.ToArray();
        if (!units.Any()) return ArraySegment<Guid>.Empty;

        var routeId = units.First().RouteId;
        if (units.Any(x => x.RouteId != routeId))
        {
            throw new BadRequestException(units.Where(x => x.RouteId != routeId)
                .Select(x => (nameof(units), (object)x)).ToArray());
        }

        var route = await _writeRepository.Read.GetByIdAsync(routeId, cancellationToken);
        if (route.UserId != userId)
        {
            throw new NotAllowedException();
        }

        await _tariffRepository.CheckExistence(units.Select(x => x.TariffId), cancellationToken);

        foreach (var unit in units)
        {
            unit.Id = Guid.NewGuid();
            await _writeRouteUnitRepository.AddAsync(unit, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return units.Select(x => x.Id);
    }

    public async Task UpdateEntityUnits(Guid userId, IEnumerable<RouteUnit> units, CancellationToken cancellationToken)
    {
        units = units as RouteUnit[] ?? units.ToArray();
        if (!units.Any()) return;

        var routeId = units.First().RouteId;

        var route = await _writeRepository.Read.GetByIdAsync(routeId, cancellationToken);
        if (route.UserId != userId)
        {
            throw new NotAllowedException();
        }

        await _tariffRepository.CheckExistence(units.Select(x => x.TariffId), cancellationToken);

        foreach (var unit in units)
        {
            var unitExist = route.RouteUnits!.FirstOrDefault(x => x.Id == unit.Id);
            if (unitExist != null)
            {
                _mapper.Map(unit, unitExist);
            }
            else
            {
                unit.Id = Guid.NewGuid();
                await _writeRouteUnitRepository.AddAsync(unit, cancellationToken);
            }
        }

        await _writeRepository.UpdateAsync(route, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteEntityUnits(Guid userId, IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        ids = ids as Guid[] ?? ids.ToArray();
        if (!ids.Any()) return;

        var routeUnitId = ids.First();

        var route = await _writeRepository.Read.GetBySpecAsync(
            new Specification<Route>(x =>
                x.RouteUnits != null && x.RouteUnits.Any(y => y.Id == routeUnitId)), cancellationToken);

        if (route.UserId != userId)
        {
            throw new NotAllowedException();
        }

        var routeUnits =
            await _writeRouteUnitRepository.Read.FilterAsync(new Specification<RouteUnit>(x => ids.Contains(x.Id)),
                cancellationToken);

        foreach (var routeUnit in routeUnits)
        {
            await _writeRouteUnitRepository.RemoveAsync(routeUnit, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}