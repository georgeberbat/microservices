using System;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ProfileDomain;
using Shared.Dal;

namespace Profile.Dal
{

    public class ProfileDbContext : BaseDbContext<ProfileDbContext>
    {
        public DbSet<User> Users => Set<User>();

#pragma warning disable CA2255
        [ModuleInitializer]
#pragma warning restore CA2255
        public static void RegisterEnums()
        {
        }
        
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);
        }
    }
}