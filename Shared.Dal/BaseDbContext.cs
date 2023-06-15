using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dex.Ef.Contracts;
using Dex.Ef.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Internal;
using Npgsql;

namespace Shared.Dal
{
    public abstract class BaseDbContext<T> : DbContext where T : DbContext
    {
        // ReSharper disable once StaticMemberInGenericType
        private static Dictionary<Type, List<Type>> RegisteredEnums { get; } = new();

        public virtual bool IsReadOnly => ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking;

        protected BaseDbContext(DbContextOptions<T> options)
            : base(options)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            if (IsReadOnly && ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.NoTracking)
            {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }

            SavingChanges += OnSavingChanges;
        }


        protected static void RegisterEnum<TEnum>() where TEnum : struct, Enum
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<TEnum>();

            if (!RegisteredEnums.TryGetValue(typeof(T), out var l))
            {
                l = new List<Type>();
                RegisteredEnums[typeof(T)] = l;
            }

            l.Add(typeof(TEnum));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            // register extensions
            modelBuilder.HasPostgresExtension("uuid-ossp");

            // register enums
            CreateEnumsModels(modelBuilder);

            // register models
            var sw = Stopwatch.StartNew();
            var modelStore = this.GetService<ModelStore<T>>();
            foreach (var modeType in modelStore.GetModels())
            {
                modelBuilder.Entity(modeType);
            }
            
            // register advanced
            modelBuilder.NormalizeEmail();
            // установка для всех моделей конкаренси токен
            modelBuilder.UseXminAsConcurrencyToken(this, GetIgnoreConcurrencyTokenTypes());
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);

            Trace.WriteLine("OnModelCreating: " + sw.Elapsed);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder == null) throw new ArgumentNullException(nameof(optionsBuilder));

            base.OnConfiguring(optionsBuilder);

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif
        }

        protected virtual Type[] GetIgnoreConcurrencyTokenTypes()
        {
            return Array.Empty<Type>();
        }

        private void CreateEnumsModels(ModelBuilder modelBuilder)
        {
            if (RegisteredEnums.TryGetValue(typeof(T), out var enums))
            {
                var nameTranslator = this.GetService<INpgsqlNameTranslator>();
                foreach (var type in enums)
                {
                    var name = nameTranslator.TranslateTypeName(type.Name);
                    var labels = Enum.GetNames(type).Select(x => nameTranslator.TranslateMemberName(x)).ToArray();
                    modelBuilder.HasPostgresEnum(name, labels);
                }
            }
        }

        private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
        {
            if (IsReadOnly) throw new NotSupportedException("context is readonly");

            var systemClock = this.GetService<ISystemClock>();
            var now = systemClock.UtcNow;
            foreach (var x in ChangeTracker.Entries())
            {
                if (x.State == EntityState.Added && x.Entity is ICreatedUtc created)
                {
                    if (created.CreatedUtc == default)
                    {
                        created.CreatedUtc = now.UtcDateTime;
                    }
                }

                if (x.State is EntityState.Added or EntityState.Modified && x.Entity is IUpdatedUtc updated)
                {
                    updated.UpdatedUtc = now.UtcDateTime;
                }
            }
        }
    }
}