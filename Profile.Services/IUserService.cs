using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProfileDomain;

namespace Profile.Services
{
    public interface IUserService
    {
        Task<ProfileDomain.User> GetUser(Guid id, CancellationToken cancellationToken);
        
        Task<RegisterUserResponse> RegisterUser(IRegisterUserCommand request, CancellationToken cancellationToken);
        Task DeleteUser(Guid userId, CancellationToken cancellationToken);
    }
}