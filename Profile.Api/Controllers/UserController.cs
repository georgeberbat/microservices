using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.Services;
using ProfileDomain;

namespace Profile.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    
    public class UserController : ControllerBase
    {
        public UserController()
        {
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}