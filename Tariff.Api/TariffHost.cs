using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using Tariff.Dal;

namespace Tariff.Api
{
    public class TariffHost : BaseHost<Startup>
    {
        protected override async Task SeedDatabase(IHost host, bool ensureDeleted)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            var config = host.Services.GetRequiredService<IConfiguration>();

            var dbSeeder = new TariffDbSeeder(config.GetConnectionString("DefaultConnection"));
            await dbSeeder.RunAsync(ensureDeleted).ConfigureAwait(false);
        }
    }
}