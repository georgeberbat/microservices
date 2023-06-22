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
                var locations = new []
                {
                    new Models.Location
                    {
                        Id = Guid.NewGuid(),
                        Name = "Location 1",
                        Address = "123 Main Street",
                        Latitude = 37.123456,
                        Longitude = -122.654321,
                        DeletedUtc = null,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    },
                    new Models.Location
                    {
                        Id = Guid.NewGuid(),
                        Name = "Location 2",
                        Address = "456 Elm Street",
                        Latitude = 38.654321,
                        Longitude = -121.123456,
                        DeletedUtc = null,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    },
                    new Models.Location
                    {
                        Id = Guid.NewGuid(),
                        Name = "Location 3",
                        Address = "789 Oak Street",
                        Latitude = 39.987654,
                        Longitude = -120.456789,
                        DeletedUtc = null,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    }
                };

                dbContext.Locations.AddRange(locations);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}