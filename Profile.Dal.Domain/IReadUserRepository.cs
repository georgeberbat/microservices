using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Domain
{
    public interface IReadUserRepository : IReadRepository<User, Guid>
    {
    }
}