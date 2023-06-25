using FakeData.Profile;
using Shared.Dal.Seeder;
using Tariff.Models;

namespace Tariff.Dal
{
    public class TariffDbSeeder : BaseEFSeeder<TariffDbContext>
    {
        public TariffDbSeeder(string connectionString)
        {
            AddDbContext(connectionString);
        }

        protected override async Task EnsureSeedData(TariffDbContext dbContext)
        {
            if (!dbContext.Tariff.Any())
            {
                var tariffs = new[]
                {
                    new Models.Tariff
                    {
                        Id = Guid.NewGuid(),
                        UserId = UserConstantId.FakeIdArray[0],
                        Name = "Tariff 1",
                        DeletedUtc = null,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    }
                };

                dbContext.Tariff.AddRange(tariffs);

                var tariffUnits = new[]
                {
                    new TariffUnit
                    {
                        Id = Guid.NewGuid(),
                        TariffId = tariffs[0].Id,
                        LocationId = Guid.NewGuid(),
                        NextLocationId = Guid.NewGuid(),
                        WeightScaleCoefficient = 1.5,
                        Distance = 100,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    }
                };

                dbContext.TariffUnit.AddRange(tariffUnits);


                var routeId = Guid.NewGuid();  
                var routes = new[]
                {
                    new Route
                    {
                        Id = routeId,
                        UserId = UserConstantId.FakeIdArray[0],
                        Name = "Route 1",
                        Description = "Route description",
                        CreatedUtc = DateTime.UtcNow,
                        RouteUnits = new List<RouteUnit>
                        {
                            new RouteUnit
                            {
                                Id = Guid.NewGuid(),
                                RouteId = routeId,
                                TariffId = tariffs[0].Id,
                                CreatedUtc = DateTime.UtcNow,
                                UpdatedUtc = DateTime.UtcNow
                            }
                        }
                    }
                };

                dbContext.Route.AddRange(routes);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}