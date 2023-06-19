using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profile.Dal;
using Profile.Dal.Migrations;
using Profile.Dal.Migrations.SecurityTokenDb;
using Shared;
using Shared.Dal;
using Shared.Password;

namespace ProfileDbSeeder
{
    public class Startup : BaseStartup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
            
            services.AddDbContext<ProfileDbContext, IProfileMigrationMarker>(Configuration.GetConnectionString("DefaultConnection"));
            services.AddDbContext<SecurityTokenDbContext, InitSecurityTokenDb>(Configuration.GetConnectionString("SecurityProviderConnection"));

            services.AddScoped<Profile.Dal.ProfileDbSeeder>();
            services.AddScoped<SecurityTokenDbSeeder>();
        }
    }
}