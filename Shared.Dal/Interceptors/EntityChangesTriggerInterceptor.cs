// ReSharper properties

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dex.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Dal.Interceptors
{
    internal sealed class EntityChangesTriggerInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityChangesTriggerInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            OnSavingChanges(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            OnSavingChanges(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void OnSavingChanges(DbContext? dbContext)
        {
            if (dbContext == null) return;

            var allTriggers = _serviceProvider.GetServices<IEntityChangesTrigger>();
            // ReSharper disable PossibleMultipleEnumeration
            if (allTriggers.IsNullOrEmpty()) return;

            foreach (var entities in dbContext.ChangeTracker.Entries().GroupBy(x => x.Metadata.ClrType))
            {
                entities.GroupBy(x => x.State).ForEach(x =>
                {
                    var entityState = x.Key;
                    foreach (var trigger in allTriggers.Where(t => t.EntityType == entities.Key))
                    {
                        trigger.RunTrigger(x.Select(ent => ent.Entity), entityState);
                    }
                });
            }
        }
    }
}