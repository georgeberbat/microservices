using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Profile.Services;
using ProfileDomain;

namespace Profile.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("get")]
        public async Task<User> GetUser(Guid id, CancellationToken cancellationToken)
        {
            return await _userService.GetUser(id, cancellationToken);
        }
    }
}