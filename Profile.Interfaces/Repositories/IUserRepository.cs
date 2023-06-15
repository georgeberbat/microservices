using System;
using System.Threading.Tasks;
using ProfileDomain;

namespace Profile.ServiceInterfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUser(Guid id);
    }
}