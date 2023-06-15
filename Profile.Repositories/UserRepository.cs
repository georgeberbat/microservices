using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Profile.Dal;
using Profile.ServiceInterfaces.Repositories;
using ProfileDomain;
using Shared.Dal.Repository;

namespace Infrastructure.Repositories
{
    public class UserRepository : BaseReadonlyRepository<ProfileDbContext>, IUserRepository
    {
        public UserRepository(ProfileDbContext dbContext, bool enableTracker = false) : base(dbContext, enableTracker)
        {
            if (!enableTracker)
            {
                DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;   
            }
        }
        
        public async Task<User?> GetUser(Guid id)
        {
            return await DbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}