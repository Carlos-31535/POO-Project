using System;
using System.Collections.ObjectModel;
using System.Linq; 
using System.Windows;
using System.Windows.Input;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Commands;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IViewFactory _viewFactory;
        private readonly IArranjoRepository _arranjoRepository;
        private readonly IFuncionarioRepository _funcionarioRepository; 

        // --- CONTROLO DE VISIBILIDADE ---
        private bool _isFuncionariosVisible = true;
        private bool _isArranjosVisible = false;
        private string _tituloTabela = "Lista de Funcionários";

        public bool IsFuncionariosVisible
        {
            get => _isFuncionariosVisible;
            set { _isFuncionariosVisible = value; OnPropertyChanged(nameof(IsFuncionariosVisible)); }
        }

        public bool IsArranjosVisible
        {
            get => _isArranjosVisible;
            set { _isArranjosVisible = value; OnPropertyChanged(nameof(IsArranjosVisible)); }
        }

        public string TituloTabela
        {
            get => _tituloTabela;
            set { _tituloTabela = value; OnPropertyChanged(nameof(TituloTabela)); }
        }

        // --- LISTAS E COMANDOS ---
        public ObservableCollection<Funcionario> Funcionarios { get; set; }
        public ObservableCollection<Arranjo> Arranjos { get; set; }

        public ICommand OpenArranjoCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ShowPendingArranjoCommand { get; }
        public ICommand ShowFinishedArranjoCommand { get; }

        public ICommand ShowFuncionariosCommand { get; }
        public ICommand MarkAsReadyCommand { get; }
        public ICommand ShowDetailsArranjoCommand { get; }
        public ICommand OpenAddFuncionarioCommand { get; }

        // --- CONSTRUTOR ---
        // Adicionei o IFuncionarioRepository aqui
        public MainViewModel(IViewFactory viewFactory,
                             IArranjoRepository arranjoRepository,
                             IFuncionarioRepository funcionarioRepository)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _arranjoRepository = arranjoRepository ?? throw new ArgumentNullException(nameof(arranjoRepository));
            _funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));

            // Inicializar Listas
            Funcionarios = new ObservableCollection<Funcionario>();
            Arranjos = new ObservableCollection<Arranjo>();

            // Inicializar Comandos
            OpenArranjoCommand = new ViewModelCommand(ExecuteOpenArranjo);
            EditCommand = new RelayCommand<Funcionario>(EditEmployee);
            ShowPendingArranjoCommand = new ViewModelCommand(ExecuteShowPendingArranjos);
            ShowFinishedArranjoCommand = new ViewModelCommand(ExecuteShowFinishedArranjos);
            ShowFuncionariosCommand = new ViewModelCommand(ExecuteShowFuncionarios);
            MarkAsReadyCommand = new RelayCommand<Arranjo>(ExecuteMarkAsReady);
            ShowDetailsArranjoCommand = new RelayCommand<Arranjo>(ExecuteShowDetailsArranjos);
            OpenAddFuncionarioCommand = new ViewModelCommand(ExecuteOpenAddFuncionario);

            // Carregar dados REAIS ao iniciar 
            CarregarFuncionarios();
        }

        // --- MÉTODOS DE DADOS ---

        private void CarregarFuncionarios()
        {
            Funcionarios.Clear();
            // Vai buscar a lista ao ficheiro JSON através do repositório
            var listaDoDisco = _funcionarioRepository.GetAll();

            foreach (var f in listaDoDisco)
            {
                Funcionarios.Add(f);
            }
        }

        // --- LÓGICA DE TROCA DE ECRA ---
        private void ExecuteShowPendingArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Pendentes (Por Arranjar)";

            var todos = _arranjoRepository.GetAllArranjos();
            var pendentes = todos.Where(a => a.Estado != EstadoArranjo.Pronto && a.Estado != EstadoArranjo.Entregue).ToList();

            Arranjos.Clear();
            foreach (var a in pendentes) Arranjos.Add(a);
        }

        private void ExecuteShowFinishedArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Prontos";

            var todos = _arranjoRepository.GetAllArranjos();
            var prontos = todos.Where(a => a.Estado == EstadoArranjo.Pronto).ToList();

            Arranjos.Clear();
            foreach (var a in prontos) Arranjos.Add(a);
        }

        private void ExecuteShowFuncionarios(object? obj)
        {
            IsFuncionariosVisible = true;
            IsArranjosVisible = false;
            TituloTabela = "Lista de Funcionários";

            // Garante que a lista está atualizada sempre que voltas a este ecrã
            CarregarFuncionarios();
        }

        // --- OUTROS MÉTODOS ---
        private void ExecuteOpenArranjo(object? parameter)
        {
            Window window = _viewFactory.ShowDialog(ViewType.AddArranjo);
            window.ShowDialog();

            if (IsArranjosVisible) ExecuteShowPendingArranjos(null);
        }

        private void EditEmployee(Funcionario funcionario)
        {
            MessageBox.Show($"A editar: {funcionario.FirstName}");
        }

        private void ExecuteMarkAsReady(Arranjo arranjo)
        {
            if (arranjo != null)
            {
                arranjo.Estado = EstadoArranjo.Pronto;
                _arranjoRepository.Update(arranjo);

                MessageBox.Show($"Talão {arranjo.Id} marcado como PRONTO! Vai mover-se para a lista de Prontos.", "Sucesso");

                if (Arranjos.Contains(arranjo))
                {
                    Arranjos.Remove(arranjo);
                }
            }
        }

        private void ExecuteShowDetailsArranjos(Arranjo arranjo)
        {
            if (arranjo == null) return;

            string detalhes = $"Cliente: {arranjo.Cliente.FirstName} {arranjo.Cliente.LastName}\n";
            detalhes += $"Data: {arranjo.DataEntrada}\n\n";
            detalhes += "ITENS PARA ARRANJAR:\n";
            detalhes += "----------------------------------\n";

            foreach (var item in arranjo.ListaCalcado)
            {
                detalhes += $"👞 {item.NumPar} ({item.Tipo} - {item.Cor})\n";
                detalhes += $"   📝 Descrição: {item.Descricao}\n";
                detalhes += $"   🛠️ Serviços: {string.Join(", ", item.ServicosParaFazer)}\n";
                detalhes += "----------------------------------\n";
            }

            MessageBox.Show(detalhes, $"Detalhes do Talão #{arranjo.Id}");
        }

        private void ExecuteOpenAddFuncionario(object? obj)
        {
            // 1. Abre a janela de criar funcionário
            Window window = _viewFactory.ShowDialog(ViewType.CreateFuncionario);

            // 2. Fica à espera que feches a janela
            window.ShowDialog();

            // 3. ASSIM QUE FECHAR, ATUALIZA A LISTA COM OS DADOS NOVOS
            CarregarFuncionarios();
        }
    }
}