using Microsoft.Extensions.DependencyInjection;
using POOProject.Models.Repositories;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels;
using POOProject.ViewModels.Interfaces;
using POOProject.ViewModels.Services;
using POOProject.Views;
using POOProject.Views.Factories;
using POOProject.Views.Interfaces;
using System.Configuration;
using System.Data;
using System.Windows;

namespace POOProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private static void LoadServiceProvider()
        {
            ServiceProvider = new ServiceCollection()
                
                .AddSingleton<LoginViewModel>()
                .AddSingleton<RegistryViewModel>()
                .AddSingleton<IViewFactory, ViewFactory>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IAuthenticationService, AuthenticationService>()

                

        
                .AddSingleton<IArranjoRepository, ArranjoRepository>()
                // .AddSingleton<IEmployeeRepository, EmployeeRepository>() // <-- Descomenta se tiveres criado o ficheiro EmployeeRepository.cs

                // 2. ViewModels Novos (Main e Arranjo)
                .AddTransient<MainViewModel>()
                .AddTransient<AddArranjoViewModel>()

                // 3. Views / Janelas (Necessário para o Factory as conseguir criar)
                .AddTransient<LoginWindow>()
                .AddTransient<RegistryWindow>()
                .AddTransient<MainWindow>()
                .AddTransient<AddArranjoWindow>()
                

                // Finaliza
                .BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadServiceProvider();
        }
    }

}
