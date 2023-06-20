using System;
using System.Threading;
using System.Threading.Tasks;
using ProfileDomain;

namespace Profile.Services
{
    public interface IUserService
    {
        Task<ProfileDomain.User> GetUser(Guid id, CancellationToken cancellationToken);
        
        Task RegisterUser(IRegisterUserCommand request, CancellationToken cancellationToken);
    }
}