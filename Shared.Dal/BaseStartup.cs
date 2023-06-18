using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Npgsql;


namespace Shared.BaseDbSeeder
{
    public abstract class BaseStartup
    {
        protected IHostEnvironment Environment { get; }
        protected IConfiguration Configuration { get; }

        protected BaseStartup(HostBuilderContext builderContext)
        {
            if (builderContext == null) throw new ArgumentNullException(nameof(builderContext));
            Environment = builderContext.HostingEnvironment;
            Configuration = builderContext.Configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
        }
    }
}