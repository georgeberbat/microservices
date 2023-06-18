using System.Threading.Tasks;

namespace gRPC.UserStore
{
    public interface IUserService<T>
    {
        /// <summary>
        /// Validates the credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public Task<bool> ValidateCredentials(string username, string password);

        /// <summary>
        /// Finds the user by subject identifier.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public Task<T> FindBySubjectId(string subjectId);

        /// <summary>
        /// Finds the user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public Task<T> FindByUsername(string username);

        /// <summary>
        /// Finds the user by external provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Task<T> FindByExternalProvider(string provider, string userId);
    }
}