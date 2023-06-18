using System.Threading.Tasks;
using AutoMapper;
using Identity.Mapping;
using Identity.SeedData;
using Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity
{
    public class IdentityHost : BaseHost<Startup>
    {
        protected override async Task SeedDatabase(IHost host, bool ensureDeleted)
        {
            var config = host.Services.GetRequiredService<IConfiguration>();

            var mapperConfig = new MapperConfiguration(expression => expression.AddProfile(new MainProfile()));
            var configurationDbSeeder = new ConfigurationDbSeeder(config.GetConnectionString("DefaultConnection"), mapperConfig.CreateMapper());
            await configurationDbSeeder.RunAsync(ensureDeleted);

            var persistedGrantDbSeeder = new PersistedGrantDbSeeder(config.GetConnectionString("DefaultConnection"));
            await persistedGrantDbSeeder.RunAsync(false);
        }
    }
}