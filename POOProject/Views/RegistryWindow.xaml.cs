
using System.Windows;
using POOProject.ViewModels;

namespace POOProject.Views
{
    /// <summary>
    /// Interaction logic for <c>LoginWindow.xaml</c>.
    /// This window handles user login and binds to <see cref="RegistryViewModel"/>.
    /// </summary>
    public partial class RegistryWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoginWindow"/>.
        /// Sets up the <see cref="RegistryViewModel"/>, authentication service, and view factory.
        /// </summary>
        public RegistryWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
