using System.Transactions;
using Dex.Cap.Outbox.Interfaces;
using Dex.Specifications;
using Location.Models.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;
using ProfileDomain.Commands;
using Shared.Dal;
using Shared.Dal.Specifications;
using Shared.Exceptions;
using Shared.MassTransit;
using Tariff.Dal.Domain.Tariff;
using Tariff.Models;
using Tariff.Services;

namespace Tariff.Async;

public class OnLocationRemovedConsumer : BaseConsumer<OnLocationRemovedCommand>
{
    private readonly ILogger<OnLocationRemovedConsumer> _logger;
    private readonly ITariffService _tariffService;
    private readonly IReadTariffUnitRepository _tariffUnitRepository;
    private readonly IReadTariffRepository _tariffRepository;
    private readonly IReadRouteRepository _routeRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly IOutboxService<IUnityOfWork> _outboxService;

    public OnLocationRemovedConsumer(ILogger<OnLocationRemovedConsumer> logger, ITariffService tariffService,
        IReadTariffUnitRepository tariffUnitRepository, IReadTariffRepository tariffRepository,
        IUnityOfWork unityOfWork, IReadRouteRepository routeRepository,
        IOutboxService<IUnityOfWork> outboxService) : base(logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tariffService = tariffService ?? throw new ArgumentNullException(nameof(tariffService));
        _tariffUnitRepository = tariffUnitRepository ?? throw new ArgumentNullException(nameof(tariffUnitRepository));
        _tariffRepository = tariffRepository ?? throw new ArgumentNullException(nameof(tariffRepository));
        _unityOfWork = unityOfWork ?? throw new ArgumentNullException(nameof(unityOfWork));
        _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
        _outboxService = outboxService ?? throw new ArgumentNullException(nameof(outboxService));
    }

    protected override async Task Process(ConsumeContext<OnLocationRemovedCommand> context)
    {
        try
        {
            await _unityOfWork.ExecuteInTransactionAsync(async token =>
                {
                    //TU by TariffId
                    var affectedUnits =
                        (await _tariffUnitRepository.FilterAsync(
                            new Specification<TariffUnit>(x =>
                                x.LocationId == context.Message.LocationId ||
                                x.NextLocationId == context.Message.LocationId
                                ),
                            token)).ToLookup(x => x.TariffId);

                    // Tariff by tariffID
                    var affectedTariffs = (await _tariffRepository.FilterAsync(
                        new AndSpecification<Models.Tariff>(new UndeletedSpecification<Models.Tariff>(),
                            new Specification<Models.Tariff>(x =>
                                affectedUnits.Select(g => g.Key).ToArray().Contains(x.Id))),
                        token)).ToDictionary(x => x.Id);

                    foreach (var group in affectedUnits)
                    {
                        var userId = affectedTariffs[group.Key].UserId;
                        await _tariffService.DeleteEntityUnits(userId, group.Select(x => x.Id).Distinct().ToArray(),
                            token);
                    }

                    var tariffIds = affectedTariffs.Keys.ToArray();

                    var affectedRoutes = await _routeRepository.FilterAsync(new Specification<Route>(x =>
                            x.RouteUnits != null &&
                            x.RouteUnits.Select(z => z.TariffId).Any(k => tariffIds.Contains(k))),
                        token);

                    var commands = affectedRoutes.Select(x => new RouteChangedCommand
                    {
                        MessageId = context.Message.LocationId,
                        UserId = x.UserId,
                        RouteId = x.Id,
                        Title = $"Route {x.Name} changed!",
                        Text = $"Location {context.Message.Name} has been deleted. Your route affected, check it out!"
                    }).ToArray();

                    foreach (var command in commands)
                    {
                        await _outboxService.EnqueueAsync(context.Message.LocationId, command, token);
                    }

                    await _unityOfWork.SaveChangesAsync(token);
                },
                _ => Task.FromResult(true), TransactionScopeOption.RequiresNew,
                cancellationToken: context.CancellationToken);
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Tariff units with location specified: {Id} were not found", context.Message.LocationId);
        }
    }
}