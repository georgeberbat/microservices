using System;
using System.Threading.Tasks;
using Profile.Dal.Model;

namespace Profile.ServiceInterfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserDb?> GetUser(Guid id);
    }
}