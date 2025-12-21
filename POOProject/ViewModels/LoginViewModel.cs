using System.Windows;
using System.Windows.Input;
using POOProject.ViewModels.Commands;
using POOProject.ViewModels.Interfaces;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IViewFactory _viewFactory;

        private string _username = string.Empty;
        private string _password = string.Empty;

        // Ação delegada à View para fechar a janela (respeitando o MVVM puro).
        public Action? HideWindowAction { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand RegistryCommand { get; }

        // Permite substituir o MessageBox real por um Mock durante os testes unitários.
        public Action<string> MessageBoxAction { get; set; } = (msg) => MessageBox.Show(msg);

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        // Injetamos os serviços necessários para não depender de implementações concretas.
        public LoginViewModel(IAuthenticationService authenticationService, IViewFactory viewFactory)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
            RegistryCommand = new ViewModelCommand(ExecuteRegistryCommand);
        }

        private void ExecuteLoginCommand(object parameter)
        {
            // Valida as credenciais contra a "base de dados"
            if (_authenticationService.Authenticate(Username, Password))
            {
                // Se sucesso, abre a janela principal e fecha a atual
                Window window = _viewFactory.ShowDialog(ViewType.Main);
                HideWindowAction?.Invoke();
                window?.Show();
            }
            else
            {
                MessageBoxAction("Username ou password incorretos.");
            }
        }

        private void ExecuteRegistryCommand(object param)
        {
            // Navega para o ecrã de criação de conta
            Window window = _viewFactory.ShowDialog(ViewType.Registry);
            HideWindowAction?.Invoke();
            window?.Show();
        }
    }
}