using POOProject.Models.Entities;

namespace POOProject.Models.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for user data access operations.
    /// </summary>
    public interface IUserRepository
    {
        #region Methods

        /// <summary>
        /// Retrieves a <see cref="User"/> by its username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        User? GetUserByUsername(string username);

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool CreateUser(string username, string password);

        #endregion
    }
}
