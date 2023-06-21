using System;
using System.Security.Authentication;
using System.Security.Claims;
using IdentityModel;

namespace Shared.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Возвращает уникальный идентификатор текущего авторизованного пользователя.
        /// </summary>
        /// <exception cref="AuthenticationException"/>
        /// <exception cref="ArgumentException"/>
        public static Guid GetUserIdAsGuid(this ClaimsPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            if (principal.Identity is not {IsAuthenticated: true})
            {
                throw new ArgumentException("Customer is not authenticated", nameof(principal));
            }

            var userIdClaim = principal.FindFirstValue(JwtClaimTypes.Subject) ??
                              principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                              throw new AuthenticationException("Unknown UserId");

            return Guid.Parse(userIdClaim);
        }
    }
}