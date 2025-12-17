namespace POOProject.ViewModels.Interfaces
{
    /// <summary>
    /// Provides authentication services for validating users.
    /// </summary>
    public interface IAuthenticationService
    {
        #region Methods

        /// <summary>
        /// Determines whether a user exists with the given username and password.
        /// </summary>
        /// <param name="username">The username to validate. Cannot be <c>null</c> or empty.</param>
        /// <param name="password">The password to validate. Cannot be <c>null</c> or empty.</param>
        /// <returns><c>true</c> if the user exists and credentials are correct; otherwise, <c>false</c>.</returns>
        bool UserExists(string username, string password);

        /// <summary>
        /// Creates new user check if exists with the given username and password and passwordRepeat.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="passwordRepeat"></param>
        /// <returns></returns>
        bool CreateUser(string username, string password, string passwordRepeat);

        POOProject.Models.Entities.Funcionario? CurrentFuncionario { get; }

        #endregion
    }
}
