using System.Threading.Tasks;
using Shared.BaseDbSeeder.Seeder;
using Shared.Dal;
using Shared.Dal.Seeder;

namespace ProfileDbSeeder
{
    public class SecurityTokenDbSeeder : BaseEFSeeder<SecurityTokenDbContext>, IDbSeeder
    {
        public SecurityTokenDbSeeder()
        {
        }
        
        protected override Task EnsureSeedData(SecurityTokenDbContext dbContext)
        {
            throw new System.NotImplementedException();
        }

        public Task RunAsync(bool ensureDeleted)
        {
            throw new System.NotImplementedException();
        }
    }
}