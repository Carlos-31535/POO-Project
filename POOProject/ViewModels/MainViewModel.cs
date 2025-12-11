
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POOProject.Models.Entities;             
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Commands;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.ViewModels
{
    /// <summary>
    /// ViewModel for main functionality. Handles navigation.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private readonly IViewFactory _viewFactory;
        private readonly IEmployeeRepository _repository;

        #endregion

        #region Properties

        /// <summary>
        /// Action to hide the associated window. Typically set by the view.
        /// </summary>
        public Action? HideWindowAction { get; set; }

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }

        // Comando novo para abrir os arranjos
        public ICommand OpenArranjoCommand { get; }

        // CORREÇÃO: Usar Funcionario em vez de Employee
        public ObservableCollection<Funcionario> Funcionarios { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/>.
        /// </summary>
        /// <param name="viewFactory">The factory to create views for navigation.</param>
        public MainViewModel(IViewFactory viewFactory, IEmployeeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));

            // CreateCommand = new ViewModelCommand(ExecuteCreateCommand);

            // Inicializar o comando de abrir arranjos
            OpenArranjoCommand = new ViewModelCommand(ExecuteOpenArranjo);

            // Inicializar a lista de Funcionários com dados de teste
            Funcionarios = new ObservableCollection<Funcionario>();

            // CORREÇÃO: Usar o construtor da tua classe Funcionario (Nome, Apelido, ID)
            Funcionarios.Add(new Funcionario("João", "Silva", 1));
            Funcionarios.Add(new Funcionario("Maria", "Santos", 2));
            Funcionarios.Add(new Funcionario("Pedro", "Ferreira", 3));

            // O comando de editar agora recebe um Funcionario
            EditCommand = new RelayCommand<Funcionario>(EditEmployee);
        }

        #endregion

        #region Methods

        private void ExecuteOpenArranjo(object? parameter)
        {
            // Abre a janela de Novo Arranjo
            Window window = _viewFactory.ShowDialog(ViewType.AddArranjo);
            window.ShowDialog();
        }

        private void ExecuteCreateCommand(object? parameter)
        {
            // Lógica para criar (se necessário)
        }

        private void EditEmployee(Funcionario funcionario)
        {
            // Exemplo: Editar o funcionário selecionado
            // Nota: Verifica se tens ViewType.EditEmployee no teu Enum
            Window window = _viewFactory.ShowDialog(ViewType.EditEmployee, funcionario);
            window?.Show();
        }

        #endregion
    }
}