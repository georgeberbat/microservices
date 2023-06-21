using ApiComposition.Api.GrpcClients;
using ApiComposition.Api.ServiceModel;
using ApiComposition.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;

namespace ApiComposition.Api.Controllers;

[AllowAnonymous]
public class RegisterController : BaseAnonymousController
{
    private readonly ProfileClient _profileClient;

    public RegisterController(ProfileClient profileClient)
    {
        _profileClient = profileClient;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(RegistrationRequest request)
    {
        var result = await _profileClient.RegisterUser(request);
        return Ok(result.UserId);
    }
}