using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using POOProject.ViewModels;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.Views.Factories
{
    /// <summary>
    /// Factory responsible for creating WPF Window instances based on the requested <see cref="ViewType"/>.
    /// This class centralizes the creation logic and ensures consistent initialization of windows.
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Public Methods

        /// <summary>
        /// Creates and returns a WPF <see cref="Window"/> corresponding to the specified <see cref="ViewType"/>.
        /// Optionally accepts a parameter used for initializing windows that require input.
        /// </summary>
        /// <param name="type">The type of view to create.</param>
        /// <param name="parameter">
        /// Optional parameter used during window construction (e.g., passing data to MainWindow).
        /// </param>
        /// <returns>A fully constructed WPF <see cref="Window"/> instance.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the specified <see cref="ViewType"/> does not have a corresponding Window.
        /// </exception>
        public Window ShowDialog(ViewType type, object? parameter = null)
        {
            Window window = type switch
            {
                ViewType.Login => _serviceProvider.GetRequiredService<LoginWindow>(),
                ViewType.Main => _serviceProvider.GetRequiredService<MainWindow>(),
                ViewType.Registry => _serviceProvider.GetRequiredService<RegistryWindow>(),
                ViewType.AddArranjo => _serviceProvider.GetRequiredService<AddArranjoWindow>(),
                ViewType.CreateFuncionario => _serviceProvider.GetRequiredService<CreateFuncionarioWindow>(),
                ViewType.DetalhesTalao => _serviceProvider.GetRequiredService<DetalhesTalaoView>(),
                _ => throw new NotImplementedException()
            };

            object viewModel = type switch
            {
                ViewType.Login => _serviceProvider.GetRequiredService<LoginViewModel>(),
                ViewType.Main => _serviceProvider.GetRequiredService<MainViewModel>(),
                ViewType.Registry => _serviceProvider.GetRequiredService<RegistryViewModel>(),
                ViewType.AddArranjo => _serviceProvider.GetRequiredService<AddArranjoViewModel>(),
                ViewType.CreateFuncionario => _serviceProvider.GetRequiredService<CreateFuncionarioViewModel>(),
                ViewType.DetalhesTalao => _serviceProvider.GetRequiredService<DetalhesTalaoViewModel>(),
                _ => throw new NotImplementedException()
            };

            window.DataContext = viewModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Fechar janelas
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

        #endregion
    }
}
