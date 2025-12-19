using Microsoft.Extensions.DependencyInjection;
using POOProject.Models.Repositories;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels;
using POOProject.ViewModels.Interfaces;
using POOProject.ViewModels.Services;
using POOProject.Views;
using POOProject.Views.Enums;
using POOProject.Views.Factories;
using POOProject.Views.Interfaces;
using System;
using System.Windows;

namespace POOProject
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private static void LoadServiceProvider()
        {
            var services = new ServiceCollection();

            // Repositórios e Serviços (Mantém estes, são importantes!)
            services.AddSingleton<IFuncionarioRepository, FuncionarioRepository>();
            services.AddSingleton<IArranjoRepository, ArranjoRepository>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IViewFactory, ViewFactory>();

            // ViewModels
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegistryViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<AddArranjoViewModel>();
            services.AddTransient<CreateFuncionarioViewModel>();
            services.AddTransient<DetalhesTalaoViewModel>();

            // Janelas
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegistryWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AddArranjoWindow>();
            services.AddTransient<CreateFuncionarioWindow>();
            services.AddTransient<DetalhesTalaoView>();

            ServiceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadServiceProvider();

            // Usar a fábrica para abrir a primeira janela
            var viewFactory = ServiceProvider.GetRequiredService<IViewFactory>();
            var loginWindow = viewFactory.ShowDialog(ViewType.Login);
            loginWindow.Show();
        }
    }
}