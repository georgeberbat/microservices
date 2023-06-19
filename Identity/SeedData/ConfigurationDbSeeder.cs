using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Dal.Seeder;
using Secret = IdentityServer4.Models.Secret;

namespace Identity.SeedData
{
    public class ConfigurationDbSeeder : BaseEFSeeder<ConfigurationDbContext>
    {
        private readonly IMapper _mapper;

        public ConfigurationDbSeeder(string connectionString, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName));
            });
        }

        protected override async Task EnsureSeedData(ConfigurationDbContext context)
        {
            Log.Debug("Clients being populated");
            foreach (var client in SeedConfig.Clients.ToList())
            {
                var exClient = context.Clients.FirstOrDefault(x => x.ClientId == client.ClientId);
                if (exClient == null)
                {
                    context.Clients.Add(client.ToEntity());
                }
                else
                {
                    context.Clients.Remove(exClient);
                    var entity = _mapper.Map(client.ToEntity(), exClient);
                    context.Clients.Add(entity);
                }
            }

            Log.Debug("IdentityResources being populated");
            foreach (var resource in SeedConfig.IdentityResources.ToList())
            {
                if (!context.IdentityResources.Any(x => x.Name == resource.Name))
                    await context.IdentityResources.AddAsync(resource.ToEntity());
            }

            Log.Debug("ApiScopes being populated");
            foreach (var resource in SeedConfig.ApiScopes.ToList())
            {
                if (!context.ApiScopes.Any(x => x.Name == resource.Name))
                    await context.ApiScopes.AddAsync(resource.ToEntity());
            }

            Log.Debug("ApiResources being populated");
            foreach (var resource in SeedConfig.ApiResources.ToList())
            {
                var apiResource = context.ApiResources
                    .Include(x => x.Scopes)
                    .Include(x => x.Secrets)
                    .FirstOrDefault(x => x.Name == resource.Name);

                if (apiResource == null)
                {
                    await context.ApiResources.AddAsync(resource.ToEntity());
                }
                else
                {
                    SeedSecrets(apiResource, resource);

                    if (apiResource.Scopes.Select(x => x.Scope).Intersect(resource.Scopes).Count() != resource.Scopes.Count)
                    {
                        apiResource.Scopes.Clear();
                        apiResource.Scopes.AddRange(resource.Scopes.Select(x => new ApiResourceScope { Scope = x }));
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        private void SeedSecrets(ApiResource apiResource, IdentityServer4.Models.ApiResource resource)
        {
            var existingSecretsCount = apiResource.Secrets?
                .Select(x => x.Value)
                .Intersect(resource.ApiSecrets.Select(x => x.Value))
                .Count() ?? 0;
            if (existingSecretsCount == resource.ApiSecrets.Count) return;

            apiResource.Secrets?.Clear();
            apiResource.Secrets ??= new List<ApiResourceSecret>();
            var secretsForAdding = resource.ApiSecrets.ToArray();
            apiResource.Secrets?.AddRange(secretsForAdding.Select(x =>
            {
                var mapped = _mapper.Map<Secret, ApiResourceSecret>(x);
                mapped.ApiResourceId = apiResource.Id;
                return mapped;
            }));
        }
    }
}