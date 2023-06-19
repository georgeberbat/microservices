using System;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Repositories
{
    public interface IReadUserRepository : IReadRepository<User, Guid>
    {
    }
}