using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Services
{
    public interface IInvalidateUserTokenService
    {
        /// <summary>
        /// Инвалидировать токены пользователей, роли или полиси которых были изменены
        /// </summary>
        /// <param name="usersGuid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvalidateToken(Guid[] usersGuid, CancellationToken cancellationToken);
    }
}