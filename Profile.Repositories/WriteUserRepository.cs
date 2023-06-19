using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Profile.Dal;
using Profile.Dal.Repositories;
using ProfileDomain;
using Shared.Dal.Repository;

namespace Infrastructure.Repositories
{
    public class WriteUserRepository : BaseReadonlyRepository<ProfileDbContext>, IWriteUserRepository
    {
        public WriteUserRepository(ProfileDbContext dbContext, bool enableTracker = false) : base(dbContext, enableTracker)
        {
            if (!enableTracker)
            {
                DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;   
            }
        }

        public IReadUserRepository Read { get; }
        public Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> RemoveAsync(User entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}