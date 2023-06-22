using ApiComposition.Api.GrpcClients;
using ApiComposition.Api.ServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;

namespace ApiComposition.Api.Controllers;

public class RegisterController : BaseAnonymousController
{
    private readonly ProfileClient _profileClient;

    public RegisterController(ProfileClient profileClient)
    {
        _profileClient = profileClient ?? throw new ArgumentNullException(nameof(profileClient));
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
}