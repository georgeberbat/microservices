using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Internal;
using Npgsql;
using Shared.Dal;

namespace Tariff.Dal
{
    /// <summary>
    /// Design factory
    /// </summary>
    public class TariffDbContextFactory : IDesignTimeDbContextFactory<TariffDbContext>
    {
        public TariffDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TariffDbContext>();
            optionsBuilder.UseNpgsql();

            return new TariffDbContext(optionsBuilder.Options, new ModelStore<TariffDbContext>());
        }
    }
}