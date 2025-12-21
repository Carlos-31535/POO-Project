using System;
using System.Windows;
using System.Windows.Input;
using POOProject.ViewModels.Commands;
using POOProject.ViewModels.Interfaces;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.ViewModels
{
    /// <summary>
    /// ViewModel responsável pelo ecrã de criação de novas contas.
    /// Gere a validação dos dados de entrada e comunica com o serviço de autenticação.
    /// </summary>
    public class RegistryViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IViewFactory _viewFactory;

        // --- CAMPOS DE DADOS (Backing Fields) ---
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _passwordRepeat = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;

        // Ação delegada para fechar a janela atual (respeita o desacoplamento do MVVM)
        public Action? HideWindowAction { get; set; }

        // Comando disparado pelo botão "Registar"
        public ICommand RegistryCommand { get; }

        // --- PROPRIEDADES (Binding) ---
        // Estas propriedades estão ligadas diretamente às TextBoxes da View (TwoWay Binding).

        public string Username { get => _username; set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string PasswordRepeat { get => _passwordRepeat; set { _passwordRepeat = value; OnPropertyChanged(nameof(PasswordRepeat)); } }

        // Nome e Apelido são obrigatórios para instanciar a classe 'Pessoa' corretamente
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(nameof(FirstName)); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(nameof(LastName)); } }

        // --- CONSTRUTOR ---

        // Recebe as dependências necessárias via Injeção de Dependência
        public RegistryViewModel(IAuthenticationService authenticationService, IViewFactory viewFactory)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            RegistryCommand = new ViewModelCommand(ExecuteRegistryCommand);
        }

        // --- LÓGICA DE REGISTO ---

        private void ExecuteRegistryCommand(object parameter)
        {
            // 1. Validações de Interface (Sanity Checks)
            // Antes de incomodar o serviço/base de dados, verificamos se os dados básicos estão preenchidos.
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("O Nome e Apelido são obrigatórios.", "Erro");
                return;
            }

            // Validação de segurança simples para garantir que o user não se enganou na password
            if (Password != PasswordRepeat)
            {
                MessageBox.Show("As passwords não coincidem.", "Erro");
                return;
            }

            // 2. Chamada ao Serviço de Domínio
            // Tenta criar o registo. O serviço encarrega-se de verificar regras mais complexas (ex: duplicados).
            if (_authenticationService.RegisterFuncionario(Username, Password, FirstName, LastName))
            {
                MessageBox.Show("Funcionário criado com sucesso!");

                // Navegação: Volta para o Login após sucesso
                Window window = _viewFactory.ShowDialog(ViewType.Login);
                HideWindowAction?.Invoke();

                // O '?' previne crashes se a Factory retornar null (comum em testes unitários)
                window?.Show();
            }
            else
            {
                MessageBox.Show("Erro ao criar conta. Verifique se o username já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}