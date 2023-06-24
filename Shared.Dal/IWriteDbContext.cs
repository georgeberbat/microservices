using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.Dal
{
    public interface IWriteDbContext : IReadDbContext
    {
        EntityEntry Add(object entity);
        EntityEntry Update(object entity);
        EntityEntry Remove(object entity);
    }
}