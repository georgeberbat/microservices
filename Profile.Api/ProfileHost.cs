using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Profile.Dal;
using Shared;
using Shared.Password;

namespace Profile
{
    public class ProfileHost : BaseHost<Startup>
    {
        protected override async Task SeedDatabase(IHost host, bool ensureDeleted)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            var config = host.Services.GetRequiredService<IConfiguration>();
            using (var scope = host.Services.CreateScope())
            {
                var passwordGenerator = scope.ServiceProvider.GetRequiredService<IPasswordGenerator>();
                var dbSeeder = new ProfileDbSeeder(config.GetConnectionString("DefaultConnection"), passwordGenerator);
                await dbSeeder.RunAsync(ensureDeleted).ConfigureAwait(false);
            }
        }
    }
}