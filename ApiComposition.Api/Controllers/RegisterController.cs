using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Controllers;

namespace ApiComposition.Api.Controllers;

[Authorize]
public class RegisterController : BaseAnonymousController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}