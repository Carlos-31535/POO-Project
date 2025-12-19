using System;
using System.Windows;
using System.Windows.Input;
using POOProject.ViewModels.Commands;
using POOProject.ViewModels.Interfaces;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.ViewModels
{
    public class RegistryViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IViewFactory _viewFactory;

        // Campos
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _passwordRepeat = string.Empty;
        // NOVOS CAMPOS
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;

        public Action? HideWindowAction { get; set; }
        public ICommand RegistryCommand { get; }

        // --- PROPRIEDADES ---
        public string Username { get => _username; set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string PasswordRepeat { get => _passwordRepeat; set { _passwordRepeat = value; OnPropertyChanged(nameof(PasswordRepeat)); } }

        // Novas Propriedades para o Binding da View
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(nameof(FirstName)); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(nameof(LastName)); } }

        public RegistryViewModel(IAuthenticationService authenticationService, IViewFactory viewFactory)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            RegistryCommand = new ViewModelCommand(ExecuteRegistryCommand);
        }

        private void ExecuteRegistryCommand(object parameter)
        {
            // 1. Validações básicas
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("O Nome e Apelido são obrigatórios.", "Erro");
                return;
            }
            if (Password != PasswordRepeat)
            {
                MessageBox.Show("As passwords não coincidem.", "Erro");
                return;
            }

            // 2. Tentar Registar o Funcionário
            if (_authenticationService.RegisterFuncionario(Username, Password, FirstName, LastName))
            {
                MessageBox.Show("Funcionário criado com sucesso!");
                Window window = _viewFactory.ShowDialog(ViewType.Login);
                HideWindowAction?.Invoke();
                window?.Show();
            }
            else
            {
                MessageBox.Show("Erro ao criar conta. Verifique se o username já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}