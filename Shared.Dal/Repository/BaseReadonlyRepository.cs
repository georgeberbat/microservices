using Microsoft.EntityFrameworkCore;

namespace Shared.Dal.Repository
{
    public abstract class BaseReadonlyRepository<TContext> where TContext : DbContext
    {
        protected readonly TContext DbContext;
        private readonly bool _enableTracker;

        protected BaseReadonlyRepository(TContext dbContext, bool enableTracker = false)
        {
            DbContext = dbContext;
            _enableTracker = enableTracker;
        }
    }
}