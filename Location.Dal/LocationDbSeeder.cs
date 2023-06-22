using Shared.Dal.Seeder;

namespace Location.Dal
{
    public class LocationDbSeeder : BaseEFSeeder<LocationDbContext>
    {
        public LocationDbSeeder(string connectionString)
        {
            AddDbContext(connectionString);
        }

        protected override async Task EnsureSeedData(LocationDbContext dbContext)
        {
            if (!dbContext.Locations.Any())
            {
            }
        }
    }
}