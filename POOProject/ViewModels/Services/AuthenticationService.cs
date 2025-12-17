using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Interfaces;
using POOProject.Models.Entities;

namespace POOProject.ViewModels.Services
{
    /// <summary>
    /// Provides authentication services to validate users.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly IUserRepository _userRepository;
        private readonly IFuncionarioRepository _funcionarioRepository;

        public Funcionario? CurrentFuncionario { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationService"/> with a user repository.
        /// </summary>
        /// <param name="userRepository">Repository used to fetch user data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="userRepository"/> is null.</exception>
        public AuthenticationService(IUserRepository userRepository, IFuncionarioRepository funcionarioRepository)
        {
            _userRepository = userRepository;
            _funcionarioRepository = funcionarioRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if a user with the specified username and password exists.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>True if a user exists and the password matches; otherwise, false.</returns>
        public bool UserExists(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            var user = _userRepository.GetUserByUsername(username);

            if (user != null && user.Password == password)
            {
                // --- FALTA ESTA LÓGICA DE CARREGAR O PERFIL ---
                var perfil = _funcionarioRepository.GetByUsername(username);

                if (perfil != null)
                    CurrentFuncionario = perfil;
                else
                    CurrentFuncionario = new Funcionario("", "", 0) { Username = username };

                return true;
            }
            return false;
        }

        public bool CreateUser(string username, string password, string passwordRepeat)
        {
            // Return false if username or password is null or empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordRepeat))
            {
                return false;
            }

            if (password == passwordRepeat)
            {
                return _userRepository.CreateUser(username, password);
            }

            return false;
        }

        #endregion
    }
}