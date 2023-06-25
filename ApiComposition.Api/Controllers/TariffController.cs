using ApiComposition.Api.GrpcClients;
using ApiComposition.Api.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;
using Shared.Interfaces;

namespace ApiComposition.Api.Controllers;

public class TariffController : BaseController
{
    private readonly TariffClient _tariffClient;
    private readonly IUserIdHttpContextService _userId;

    public TariffController(TariffClient tariffClient, IUserIdHttpContextService userId)
    {
        _tariffClient = tariffClient ?? throw new ArgumentNullException(nameof(tariffClient));
        _userId = userId ?? throw new ArgumentNullException(nameof(userId));
    }

    [HttpGet]
    public async Task<IActionResult> GetTariffs(CancellationToken cancellationToken)
    {
        var response =
            await _tariffClient.GetTariffs(new GetTariffsRequestGrpc { UserId = _userId.UserId.ToString() },
                cancellationToken);

        return Ok(response.Tariffs);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTariff(TariffGrpc tariff,
        CancellationToken cancellationToken)
    {
        var response =
            await _tariffClient.CreateTariff(new CreateTariffRequestGrpc
                {
                    Tariff = tariff,
                    UserId = _userId.UserId.ToString()
                },
                cancellationToken);

        return Ok(response.TariffId);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTariff(TariffGrpc tariff,
        CancellationToken cancellationToken)
    {
        await _tariffClient.UpdateTariff(new UpdateTariffRequestGrpc
            {
                Tariff = tariff,
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTariff(Guid tariffId,
        CancellationToken cancellationToken)
    {
        await _tariffClient.DeleteTariff(new DeleteTariffRequestGrpc
            {
                TariffId = tariffId.ToString(),
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTariffUnits(ArrayRequest<TariffUnitGrpc> tariffUnits,
        CancellationToken cancellationToken)
    {
        var response =
            await _tariffClient.CreateTariffUnits(new CreateTariffUnitsRequestGrpc
                {
                    TariffUnits = { tariffUnits.Items },
                    UserId = _userId.UserId.ToString()
                },
                cancellationToken);

        return Ok(response.UnitIds.ToArray());
    }


    [HttpPut]
    public async Task<IActionResult> UpdateTariffUnits(ArrayRequest<TariffUnitGrpc> tariffUnits,
        CancellationToken cancellationToken)
    {
        await _tariffClient.UpdateTariffUnits(new UpdateTariffUnitsRequestGrpc
            {
                TariffUnits = { tariffUnits.Items },
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTariffUnits(IEnumerable<Guid> tariffUnitIds,
        CancellationToken cancellationToken)
    {
        await _tariffClient.DeleteTariffUnits(new DeleteTariffUnitsRequestGrpc
            {
                UnitIds = { tariffUnitIds.Select(x => x.ToString()).ToArray() },
                UserId = _userId.UserId.ToString()
            },
            cancellationToken);

        return Ok();
    }
}