using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Internal;
using Npgsql;
using Shared.Dal;

namespace Profile.Dal
{
    /// <summary>
    /// Design factory
    /// </summary>
    public class ProfileDbContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
    {
        public ProfileDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProfileDbContext>();
            const string? connectionString = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=my-pass~003;Database=LPRO-Profile.Api;Pooling=true;ConnectionIdleLifetime=300;MinPoolSize=15;MaxPoolSize=100;CommandTimeout=10;Timeout=5;";
            optionsBuilder.UseNpgsql(connectionString);

            return new ProfileDbContext(optionsBuilder.Options);
        }
    }
}