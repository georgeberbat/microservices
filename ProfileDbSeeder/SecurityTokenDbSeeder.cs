using System.Threading.Tasks;
using Shared.BaseDbSeeder.Seeder;
using Shared.Dal;

namespace ProfileDbSeeder
{
    public class SecurityTokenDbSeeder : BaseEFSeeder<SecurityTokenDbContext>, IDbSeeder
    {
        public SecurityTokenDbSeeder(SecurityTokenDbContext dbContext) : base(dbContext)
        {
        }

        protected override Task EnsureSeedData()
        {
            return Task.CompletedTask;
        }
    }
}