using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GorodPay.Shared.Dal.Extensions
{
    // ReSharper disable once UnusedMember.Global
    public static class ModelBuilderExtensions
    {
        public static void UseXminAsConcurrencyToken(this ModelBuilder modelBuilder, params Type[] ignoreTypes)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                if (ignoreTypes.Contains(entityType.ClrType))
                    continue;

                modelBuilder.Entity(entityType.ClrType).UseXminAsConcurrencyToken();
            }
        }

        /// <summary>
        /// Находит все колонки типа <see cref="DateTime"/> в моделях текущего контекста и добавляет им конвертер.
        /// Для хранения в базе дата будет предварительно преобразована в UTC.
        /// При выборке даты из базы, ей будет принудительно устанавлен указанный <paramref name="fetchKind"/>.
        /// </summary>
        /// <remarks>Когда дата хранится в БД как <c>timestamp</c> то у неё теряется изначальный Kind.</remarks>
        /// <param name="fetchKind">Kind для даты который будет установлен при выборке <c>timestamp</c> вместо дефолтного <see cref="DateTimeKind.Unspecified"/>.</param>
        public static void SetDefaultDateTimeKind(this ModelBuilder modelBuilder, DateTimeKind fetchKind)
        {
            UseValueConverterForType<DateTime>(modelBuilder, new DateTimeKindValueConverter(fetchKind));
            UseValueConverterForType<DateTime?>(modelBuilder, new DateTimeKindValueConverter(fetchKind));
        }

        private static void UseValueConverterForType<T>(ModelBuilder modelBuilder, ValueConverter converter)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            UseValueConverterForType(modelBuilder, typeof(T), converter);
        }

        private static void UseValueConverterForType(ModelBuilder modelBuilder, Type type, ValueConverter converter)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // note that entityType.GetProperties() will throw an exception, so we have to use reflection
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == type);

                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(converter);
                }
            }
        }

        private sealed class DateTimeKindValueConverter : ValueConverter<DateTime, DateTime>
        {
            public DateTimeKindValueConverter(DateTimeKind kind, ConverterMappingHints? mappingHints = null)
                : base(
                    v => v.ToUniversalTime(), // Что-бы в timestamp дата всегда хранилась в UTC.
                    v => DateTime.SpecifyKind(v, kind)
                        .ToLocalTime(), // timestamp в базе эквивалентен `Unspecified DateTime` поэтому просто восстанавливаем заведомо известный Kind.
                    mappingHints)
            {
            }
        }
    }
}