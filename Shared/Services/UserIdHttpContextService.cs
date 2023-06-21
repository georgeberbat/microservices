using System;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using Shared.Extensions;
using Shared.Interfaces;

namespace Shared.Services
{
    /// <summary>
    /// Для получения идентификатора текущего пользователя.
    /// </summary>
    public sealed class UserIdHttpContextService : IUserIdHttpContextService
    {
        private readonly IPrincipal _current;

        public UserIdHttpContextService(IPrincipal current)
        {
            _current = current ?? throw new ArgumentNullException(nameof(current));
        }

        /// <exception cref="AuthenticationException"/>
        public Guid UserId => ((ClaimsPrincipal) _current).GetUserIdAsGuid();
    }
}