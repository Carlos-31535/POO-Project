
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using POOProject.ViewModels;

namespace POOProject.Views
{
    /// <summary>
    /// Interaction logic for <c>LoginWindow.xaml</c>.
    /// This window handles user login and binds to <see cref="LoginViewModel"/>.
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoginWindow"/>.
        /// Sets up the <see cref="LoginViewModel"/>, authentication service, and view factory.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
