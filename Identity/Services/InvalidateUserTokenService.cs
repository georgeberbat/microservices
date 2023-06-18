using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class InvalidateUserTokenService : IInvalidateUserTokenService
    {
        private readonly PersistedGrantDbContext _persistedGrantDbContext;

        public InvalidateUserTokenService(PersistedGrantDbContext persistedGrantDbContext)
        {
            _persistedGrantDbContext = persistedGrantDbContext ?? throw new ArgumentNullException(nameof(persistedGrantDbContext));
        }

        public async Task InvalidateToken(Guid[] usersGuid, CancellationToken cancellationToken)
        {
            if (usersGuid == null || !usersGuid.Any()) throw new ArgumentNullException(nameof(usersGuid));

            var tokens = await _persistedGrantDbContext.PersistedGrants
                .Where(x => usersGuid.Select(y => y.ToString()).Contains(x.SubjectId))
                .ToListAsync(cancellationToken);

            _persistedGrantDbContext.RemoveRange(tokens);

            await _persistedGrantDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}