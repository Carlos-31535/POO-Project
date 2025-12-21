using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using POOProject.ViewModels;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.Views.Factories
{
    /// <summary>
    /// Fábrica responsável por instanciar todas as janelas e os seus respetivos ViewModels.
    /// Centraliza a lógica de criação (Pattern Factory) e resolve dependências automaticamente.
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        // O "saco" de serviços onde estão registados todos os ViewModels e Repositórios
        private readonly IServiceProvider _serviceProvider;

        public ViewFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Window ShowDialog(ViewType type, object? parameter = null)
        {
            // 1. Criar a Janela (View)
            // O GetRequiredService vai ao App.xaml.cs procurar a configuração desta janela
            Window window = type switch
            {
                ViewType.Login => _serviceProvider.GetRequiredService<LoginWindow>(),
                ViewType.Main => _serviceProvider.GetRequiredService<MainWindow>(),
                ViewType.Registry => _serviceProvider.GetRequiredService<RegistryWindow>(),
                ViewType.AddArranjo => _serviceProvider.GetRequiredService<AddArranjoWindow>(),
                ViewType.CreateFuncionario => _serviceProvider.GetRequiredService<CreateFuncionarioWindow>(),
                ViewType.DetalhesTalao => _serviceProvider.GetRequiredService<DetalhesTalaoView>(),
                _ => throw new NotImplementedException("Janela não configurada na Factory.")
            };

            // 2. Criar o Cérebro (ViewModel) correspondente
            object viewModel = type switch
            {
                ViewType.Login => _serviceProvider.GetRequiredService<LoginViewModel>(),
                ViewType.Main => _serviceProvider.GetRequiredService<MainViewModel>(),
                ViewType.Registry => _serviceProvider.GetRequiredService<RegistryViewModel>(),
                ViewType.AddArranjo => _serviceProvider.GetRequiredService<AddArranjoViewModel>(),
                ViewType.CreateFuncionario => _serviceProvider.GetRequiredService<CreateFuncionarioViewModel>(),
                ViewType.DetalhesTalao => _serviceProvider.GetRequiredService<DetalhesTalaoViewModel>(),
                _ => throw new NotImplementedException("ViewModel não configurado.")
            };

            // 3. Ligar os dois (DataBinding)
            window.DataContext = viewModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // 4. Configurar o fecho da janela (MVVM Puro)
            // Como o ViewModel não pode fechar a janela diretamente (não conhece a View),
            // passamos uma Action que o ViewModel pode chamar quando quiser fechar-se.
            if (viewModel is LoginViewModel loginVm)
            {
                loginVm.HideWindowAction = window.Close;
            }
            else if (viewModel is RegistryViewModel registryVm)
            {
                registryVm.HideWindowAction = window.Close;
            }
            else if (viewModel is AddArranjoViewModel addArranjoVm)
            {
                addArranjoVm.HideWindowAction = window.Close;
            }
            else if (viewModel is CreateFuncionarioViewModel createVm)
            {
                createVm.HideWindowAction = window.Close;
            }
            else if (viewModel is DetalhesTalaoViewModel detalhesVm)
            {
                detalhesVm.HideWindowAction = window.Close;
            }

            return window;
        }
    }
}