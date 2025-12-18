using POOProject.Models.Entities;
using POOProject.Models.Enums;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Commands;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace POOProject.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IViewFactory _viewFactory;
        private readonly IArranjoRepository _arranjoRepository;
        private readonly IFuncionarioRepository _funcionarioRepository;

        // --- NOVO: Ação para mensagens (Testável!) ---
        public Action<string> MessageBoxAction { get; set; } = (msg) => MessageBox.Show(msg);

        // --- CONTROLO DE VISIBILIDADE ---
        private bool _isFuncionariosVisible = true;
        private bool _isArranjosVisible = false;
        private string _tituloTabela = "Lista de Funcionários";

        // --- PESQUISA ---
        private string _searchText = string.Empty;
        // Cache para guardar a lista completa enquanto filtramos a visual
        private List<Arranjo> _listaCompletaCache = new List<Arranjo>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FiltrarLista(); // Filtra ao escrever
            }
        }

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

        // --- LISTAS ---
        public ObservableCollection<Funcionario> Funcionarios { get; set; }
        public ObservableCollection<Arranjo> Arranjos { get; set; }

        // --- COMANDOS ---
        public ICommand OpenArranjoCommand { get; }
        public ICommand ShowPendingArranjoCommand { get; }
        public ICommand ShowFinishedArranjoCommand { get; }
        public ICommand ShowFuncionariosCommand { get; }
        public ICommand MarkAsReadyCommand { get; }
        public ICommand ShowDetailsArranjoCommand { get; }
        public ICommand OpenAddFuncionarioCommand { get; }

        public MainViewModel(IViewFactory viewFactory,
                             IArranjoRepository arranjoRepository,
                             IFuncionarioRepository funcionarioRepository)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _arranjoRepository = arranjoRepository ?? throw new ArgumentNullException(nameof(arranjoRepository));
            _funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));

            Funcionarios = new ObservableCollection<Funcionario>();
            Arranjos = new ObservableCollection<Arranjo>();

            // Inicialização dos Comandos
            OpenArranjoCommand = new ViewModelCommand(ExecuteOpenArranjo);
            ShowPendingArranjoCommand = new ViewModelCommand(ExecuteShowPendingArranjos);
            ShowFinishedArranjoCommand = new ViewModelCommand(ExecuteShowFinishedArranjos);
            ShowFuncionariosCommand = new ViewModelCommand(ExecuteShowFuncionarios);
            MarkAsReadyCommand = new RelayCommand<Arranjo>(ExecuteMarkAsReady);
            ShowDetailsArranjoCommand = new RelayCommand<Arranjo>(ExecuteShowDetailsArranjos);
            OpenAddFuncionarioCommand = new ViewModelCommand(ExecuteOpenAddFuncionario);

            // Carrega dados iniciais
            CarregarFuncionariosReais();
        }

        // --- LÓGICA DE PESQUISA ---
        private void FiltrarLista()
        {
            if (!IsArranjosVisible) return;

            Arranjos.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Se não há pesquisa, repõe tudo o que estava em cache
                foreach (var item in _listaCompletaCache) Arranjos.Add(item);
            }
            else
            {
                string termo = SearchText.ToLower();
                var filtrados = _listaCompletaCache.Where(a =>
                    a.Cliente.NomeCompleto.ToLower().Contains(termo) ||
                    a.Id.ToLower().Contains(termo)
                );

                foreach (var item in filtrados) Arranjos.Add(item);
            }
        }

        private void CarregarFuncionariosReais()
        {
            Funcionarios.Clear();
            var listaDoDisco = _funcionarioRepository.GetAll();
            foreach (var f in listaDoDisco) Funcionarios.Add(f);
        }

        // --- LÓGICA DE NAVEGAÇÃO ---

        private void ExecuteShowPendingArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Pendentes";
            SearchText = ""; // Limpa pesquisa anterior

            var todos = _arranjoRepository.GetAllArranjos();
            // Filtra apenas os que NÃO estão prontos nem entregues
            _listaCompletaCache = todos.Where(a => a.Estado != EstadoArranjo.Pronto && a.Estado != EstadoArranjo.Entregue).ToList();

            FiltrarLista();
        }

        private void ExecuteShowFinishedArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Prontos";
            SearchText = "";

            var todos = _arranjoRepository.GetAllArranjos();
            // Filtra apenas os PRONTOS
            _listaCompletaCache = todos.Where(a => a.Estado == EstadoArranjo.Pronto).ToList();

            FiltrarLista();
        }

        private void ExecuteShowFuncionarios(object? obj)
        {
            IsFuncionariosVisible = true;
            IsArranjosVisible = false;
            TituloTabela = "Lista de Funcionários";
            CarregarFuncionariosReais();
        }

        // --- AÇÕES PRINCIPAIS ---

        private void ExecuteOpenArranjo(object? parameter)
        {
            Window window = _viewFactory.ShowDialog(ViewType.AddArranjo);
            window.ShowDialog();

            // Se estivermos a ver a lista de arranjos, atualiza-a ao fechar a janela
            if (IsArranjosVisible) ExecuteShowPendingArranjos(null);
        }

        private void ExecuteOpenAddFuncionario(object? obj)
        {
            Window window = _viewFactory.ShowDialog(ViewType.CreateFuncionario);
            window.ShowDialog();
            CarregarFuncionariosReais();
        }

        private void ExecuteMarkAsReady(Arranjo arranjo)
        {
            if (arranjo == null) return;

            try
            {
                // Lógica de negócio segura
                arranjo.MarcarComoPronto();

                // Gravar na Base de Dados
                _arranjoRepository.Update(arranjo);

                // Usar a Action em vez de MessageBox direto (para testes)
                MessageBoxAction($"Talão {arranjo.Id} marcado como PRONTO!");

                // Remove da lista atual para dar feedback imediato
                if (Arranjos.Contains(arranjo)) Arranjos.Remove(arranjo);
                if (_listaCompletaCache.Contains(arranjo)) _listaCompletaCache.Remove(arranjo);
            }
            catch (Exception ex)
            {
                MessageBoxAction($"Não foi possível concluir: {ex.Message}");
            }
        }

        private void ExecuteShowDetailsArranjos(Arranjo arranjo)
        {
            if (arranjo == null) return;

            // Usa a Factory para criar a janela de detalhes
            Window window = _viewFactory.ShowDialog(ViewType.DetalhesTalao);

            // Injeta os dados no ViewModel que foi criado pela Factory
            if (window.DataContext is DetalhesTalaoViewModel vm)
            {
                vm.CarregarDados(arranjo);
            }

            window.ShowDialog();
        }
    }
}