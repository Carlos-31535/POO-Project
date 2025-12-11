
using Microsoft.Extensions.DependencyInjection;
using POOProject.ViewModels;
using System.Windows;

namespace POOProject.Views
{
    /// <summary>
    /// Interaction logic for <c>LoginWindow.xaml</c>.
    /// This window handles user login and binds to <see cref="RegistryViewModel"/>.
    /// </summary>
    public partial class RegistryWindow : Window
    {
        private readonly RegistryViewModel RegistryView;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoginWindow"/>.
        /// Sets up the <see cref="RegistryViewModel"/>, authentication service, and view factory.
        /// </summary>
        public RegistryWindow()
        {
            RegistryView = App.ServiceProvider.GetRequiredService<RegistryViewModel>();
            InitializeComponent();
            DataContext = RegistryView;
        }

        #endregion
    }
}
