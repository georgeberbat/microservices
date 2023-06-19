using Dex.Ef.Contracts.Entities;
using Dex.Specifications;

namespace Shared.Dal.Specifications
{
    /// <summary>
    /// Спецификация для выборки неудаленных сущностей
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    public class UndeletedSpecification<TEntity> : Specification<TEntity> where TEntity : IDeletable
    {
        public UndeletedSpecification() : base(x => !x.DeletedUtc.HasValue)
        {
        }
    }
}