using System;
using Profile.Dal.Domain;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal.Repositories
{
    internal class ReadUserRepository : GenericReadRepository<User, Guid>, IReadUserRepository
    {
        public ReadUserRepository(IReadDbContext readDbContext)
            : base(readDbContext)
        {
        }
    }
}