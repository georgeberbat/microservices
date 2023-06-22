using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Attributes;

namespace Shared.Controllers
{
    [ApiController]
    [Authorize("ApiScopeRequired")]
    [Route("[controller]/[action]")]
    [ValidateModelState]
    [Produces("application/json")]
    [ProducesResponseType(typeof(int), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(int), StatusCodes.Status403Forbidden)]
    public abstract class BaseController : ControllerBase
    {
    }
}