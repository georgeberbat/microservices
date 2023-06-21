using System;
using System.Security.Authentication;

namespace Shared.Interfaces;

public interface IUserIdHttpContextService
{
    /// <summary>
    /// Идентификатор контакта (пользователя)
    /// </summary>
    /// <exception cref="AuthenticationException"/>
    Guid UserId { get; }
}