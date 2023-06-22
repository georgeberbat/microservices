using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Internal;
using Npgsql;
using Shared.Dal;

namespace Location.Dal
{
    /// <summary>
    /// Design factory
    /// </summary>
    public class LocationDbContextFactory : IDesignTimeDbContextFactory<LocationDbContext>
    {
        public LocationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LocationDbContext>();
            optionsBuilder.UseNpgsql();

            return new LocationDbContext(optionsBuilder.Options, new ModelStore<LocationDbContext>());
        }
    }
}