using Shared.Dal.Seeder;

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
                var Tariffs = new []
                {
                    new Models.Tariff
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tariff 1",
                        DeletedUtc = null,
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow
                    }
                };

                dbContext.Tariff.AddRange(Tariffs);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}