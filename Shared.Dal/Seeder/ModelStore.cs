using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dex.Ef.Contracts;

namespace Shared.Dal.Seeder
{
    /// <summary>
    /// Возвращает список моделей данных
    /// </summary>
    /// <typeparam name="T">Тип указывающий на сборку с моделями данных</typeparam>
    public class ModelStore<T> : IModelStore
    {
        private readonly Type[] _modelTypes;

        public ModelStore()
        {
            var domainAssembly = typeof(T).Assembly;

            _modelTypes = domainAssembly.GetExportedTypes()
                .Where(x => !x.IsInterface && x.GetCustomAttribute<TableAttribute>() != null)
                .ToArray();
        }

        public IEnumerable<Type> GetModels()
        {
            return _modelTypes;
        }
    }
}