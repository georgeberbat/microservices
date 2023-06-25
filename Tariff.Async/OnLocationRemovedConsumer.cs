using System.Transactions;
using Dex.Specifications;
using Location.Models.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;
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
    private readonly IUnityOfWork _unityOfWork;

    public OnLocationRemovedConsumer(ILogger<OnLocationRemovedConsumer> logger, ITariffService tariffService,
        IReadTariffUnitRepository tariffUnitRepository, IReadTariffRepository tariffRepository,
        IUnityOfWork unityOfWork) : base(logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tariffService = tariffService ?? throw new ArgumentNullException(nameof(tariffService));
        _tariffUnitRepository = tariffUnitRepository ?? throw new ArgumentNullException(nameof(tariffUnitRepository));
        _tariffRepository = tariffRepository ?? throw new ArgumentNullException(nameof(tariffRepository));
        _unityOfWork = unityOfWork ?? throw new ArgumentNullException(nameof(unityOfWork));
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
                            new Specification<TariffUnit>(x => x.LocationId == context.Message.LocationId),
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