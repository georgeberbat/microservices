using System;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Repositories
{
    public interface IWriteUserRepository : IWriteRepository<User, Guid, IReadUserRepository>
    {
    }
}