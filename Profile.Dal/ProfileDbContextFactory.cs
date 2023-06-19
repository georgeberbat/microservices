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
            optionsBuilder.UseNpgsql();

            return new ProfileDbContext(optionsBuilder.Options, new ModelStore<ProfileDbContext>());
        }
    }
}