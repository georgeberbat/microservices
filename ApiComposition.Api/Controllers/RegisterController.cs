using ApiComposition.Api.GrpcClients;
using ApiComposition.Api.ServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;
using Shared.Interfaces;

namespace ApiComposition.Api.Controllers;

public class RegisterController : BaseAnonymousController
{
    private readonly ProfileClient _profileClient;
    private readonly IUserIdHttpContextService _userIdService;

    public RegisterController(ProfileClient profileClient, IUserIdHttpContextService userIdService)
    {
        _profileClient = profileClient ?? throw new ArgumentNullException(nameof(profileClient));
        _userIdService = userIdService ?? throw new ArgumentNullException(nameof(userIdService));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser(RegistrationRequest request, CancellationToken cancellationToken)
    {
        var result = await _profileClient.RegisterUser(request, cancellationToken);
        return Ok(result.UserId);
    }
    
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteMyself(CancellationToken cancellationToken)
    {
        await _profileClient.DeleteMyself(cancellationToken);
        return Ok();
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNotifications(bool viewed, CancellationToken cancellationToken)
    {
        var userId = _userIdService.UserId;
        var mapped = new GetNotificationsRequest
        {
            Viewed = viewed,
            UserId = userId.ToString()
        };

        var response = await _profileClient.GetNotifications(mapped, cancellationToken);

        return Ok(response);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(ArrayRequest<Guid> request, CancellationToken cancellationToken)
    {
        var userId = _userIdService.UserId;
        var mapped = new MarkAsReadRequest
        {
            Ids = { request.Items.Select(x => x.ToString()).ToArray() },
            UserId = userId.ToString()
        };

        await _profileClient.MarkAsRead(mapped, cancellationToken);
        return Ok();
    }
}