using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Profile.Dal;
using Profile.Dal.Model;
using Profile.ServiceInterfaces.Repositories;
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
        
        public async Task<UserDb?> GetUser(Guid id)
        {
            return await DbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}