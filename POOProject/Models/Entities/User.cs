namespace POOProject.Models.Entities
{
    /// <summary>
    /// Represents a user of the application with login credentials.
    /// </summary>
    public class User
    {
        #region Properties

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; }

        #endregion
    }
}
