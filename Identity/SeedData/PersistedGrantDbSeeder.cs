using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Shared.Dal.Seeder;

namespace Identity.SeedData
{
    public class PersistedGrantDbSeeder : BaseEFSeeder<PersistedGrantDbContext>
    {
        public PersistedGrantDbSeeder(string connectionString)
        {
            Services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName));
            });
        }

        protected override Task EnsureSeedData(PersistedGrantDbContext dbContext)
        {
            throw new System.NotImplementedException();
        }
    }
}