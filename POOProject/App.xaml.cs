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
    /// <summary>
    /// Ponto de entrada da aplicação (Composition Root).
    /// É aqui que configuramos todo o sistema de Injeção de Dependência antes de abrir a primeira janela.
    /// </summary>
    public partial class App : Application
    {
        // O "Contentor" que vai guardar todas as peças do nosso sistema (Serviços, Repositórios, ViewModels).
        // Fica estático para poder ser acedido pela Factory em qualquer lugar.
        public static IServiceProvider ServiceProvider { get; private set; }

        private static void LoadServiceProvider()
        {
            var services = new ServiceCollection();

            // --- 1. INFRAESTRUTURA (Singletons) ---
            // Usamos Singleton para objetos que devem manter o estado durante toda a vida da aplicação.
            // Ex: O Repositório de Funcionários não pode ser recriado a cada clique, senão perdemos os dados em memória.

            services.AddSingleton<IFuncionarioRepository, FuncionarioRepository>();
            services.AddSingleton<IArranjoRepository, ArranjoRepository>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // A Factory também é única e serve a app toda.
            services.AddSingleton<IViewFactory, ViewFactory>();

            // --- 2. VIEWMODELS ---
            // Login e Registry podem ser Singleton (estado pouco complexo) ou Transient.
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegistryViewModel>();

            // Transient: Importante! Cria uma nova instância CADA vez que pedimos.
            // Queremos que o 'AddArranjo' venha limpo sempre que abrimos a janela, e não com dados do cliente anterior.
            services.AddTransient<MainViewModel>();
            services.AddTransient<AddArranjoViewModel>();
            services.AddTransient<CreateFuncionarioViewModel>();
            services.AddTransient<DetalhesTalaoViewModel>();

            // --- 3. JANELAS (Views) ---
            // As janelas WPF devem ser sempre Transient porque, depois de fechadas (.Close()), 
            // não podem ser reabertas. Precisamos sempre de uma nova.
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegistryWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AddArranjoWindow>();
            services.AddTransient<CreateFuncionarioWindow>();
            services.AddTransient<DetalhesTalaoView>();

            // Constrói o contentor final pronto a usar
            ServiceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Inicializa o "motor" de dependências
            LoadServiceProvider();

            // 2. Arranca a aplicação usando a Factory
            // Em vez de 'new LoginWindow()', pedimos à Factory para criar a janela inicial.
            // Isto garante que o LoginViewModel e os seus Serviços são injetados corretamente logo no início.
            var viewFactory = ServiceProvider.GetRequiredService<IViewFactory>();
            var loginWindow = viewFactory.ShowDialog(ViewType.Login);

            loginWindow.Show();
        }
    }
}