using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Produces("application/json")]
[ProducesResponseType(typeof(int), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(int), StatusCodes.Status403Forbidden)]
public abstract class BaseAnonymousController : ControllerBase
{
}