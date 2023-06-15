using System;
using System.Threading.Tasks;
using ProfileDomain;

namespace Profile.ServiceInterfaces.Services
{
    public interface IUserService
    {
        Task<User> GetUser(Guid id);
    }
}