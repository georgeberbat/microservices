using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Domain
{
    public interface IWriteUserRepository : IWriteRepository<User, Guid, IReadUserRepository>
    {
    }
}