using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Profile.Dal;
using Profile.Dal.Migrations;
using Profile.Dal.Migrations.SecurityTokenDb;
using Shared.BaseDbSeeder;
using Shared.Dal;
using Shared.Password;

namespace ProfileDbSeeder
{
    public class Startup : BaseStartup
    {
        public Startup(HostBuilderContext builderContext) : base(builderContext)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
            
            services.AddDbContext<ProfileDbContext, IProfileMigrationMarker>(Configuration.GetConnectionString("DefaultConnection"));
            services.AddDbContext<SecurityTokenDbContext, InitSecurityTokenDb>(Configuration.GetConnectionString("SecurityProviderConnection"));

            services.AddScoped<ProfileDbSeeder>();
            services.AddScoped<SecurityTokenDbSeeder>();
        }
    }
}