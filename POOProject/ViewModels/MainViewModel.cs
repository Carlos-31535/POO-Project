using System;
using System.Collections.Generic; // Necessário para List<>
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

        // --- PESQUISA (NOVO) ---
        private string _searchText = string.Empty;
        // Esta lista serve de "Backup" da lista original carregada da base de dados
        private List<Arranjo> _listaCompletaCache = new List<Arranjo>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                // Sempre que escreves uma letra, ele filtra automaticamente
                FiltrarLista();
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

            OpenArranjoCommand = new ViewModelCommand(ExecuteOpenArranjo);
            ShowPendingArranjoCommand = new ViewModelCommand(ExecuteShowPendingArranjos);
            ShowFinishedArranjoCommand = new ViewModelCommand(ExecuteShowFinishedArranjos);
            ShowFuncionariosCommand = new ViewModelCommand(ExecuteShowFuncionarios);
            MarkAsReadyCommand = new RelayCommand<Arranjo>(ExecuteMarkAsReady);
            ShowDetailsArranjoCommand = new RelayCommand<Arranjo>(ExecuteShowDetailsArranjos);
            OpenAddFuncionarioCommand = new ViewModelCommand(ExecuteOpenAddFuncionario);

            CarregarFuncionariosReais();
        }

        // --- LÓGICA DE PESQUISA (NOVO) ---
        private void FiltrarLista()
        {
            // Se não estamos a ver arranjos, não faz nada
            if (!IsArranjosVisible) return;

            Arranjos.Clear();

            // Se a caixa de pesquisa estiver vazia, mostra tudo o que estava em cache
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var item in _listaCompletaCache) Arranjos.Add(item);
            }
            else
            {
                // Pesquisa inteligente (Ignora maiúsculas/minúsculas)
                string termo = SearchText.ToLower();

                var filtrados = _listaCompletaCache.Where(a =>
                    a.Cliente.NomeCompleto.ToLower().Contains(termo) || // Pesquisa por Nome
                    a.Id.ToLower().Contains(termo)                      // Pesquisa por ID do Talão (opcional)
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

            // Limpa a pesquisa antiga quando mudas de aba
            SearchText = "";

            var todos = _arranjoRepository.GetAllArranjos();
            // Guarda na memória (Cache) apenas os pendentes
            _listaCompletaCache = todos.Where(a => a.Estado != EstadoArranjo.Pronto && a.Estado != EstadoArranjo.Entregue).ToList();

            FiltrarLista(); // Atualiza o ecrã
        }

        private void ExecuteShowFinishedArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Prontos";

            SearchText = "";

            var todos = _arranjoRepository.GetAllArranjos();
            // Guarda na memória (Cache) apenas os prontos
            _listaCompletaCache = todos.Where(a => a.Estado == EstadoArranjo.Pronto).ToList();

            FiltrarLista(); // Atualiza o ecrã
        }

        private void ExecuteShowFuncionarios(object? obj)
        {
            IsFuncionariosVisible = true;
            IsArranjosVisible = false;
            TituloTabela = "Lista de Funcionários";
            CarregarFuncionariosReais();
        }

        private void ExecuteOpenArranjo(object? parameter)
        {
            Window window = _viewFactory.ShowDialog(ViewType.AddArranjo);
            window.ShowDialog();
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
            if (arranjo != null)
            {
                arranjo.Estado = EstadoArranjo.Pronto;
                _arranjoRepository.Update(arranjo);

                MessageBox.Show($"Talão {arranjo.Id} marcado como PRONTO!", "Sucesso");

                // Atualiza a lista visualmente
                if (Arranjos.Contains(arranjo)) Arranjos.Remove(arranjo);
                if (_listaCompletaCache.Contains(arranjo)) _listaCompletaCache.Remove(arranjo);
            }
        }

        private void ExecuteShowDetailsArranjos(Arranjo arranjo)
        {
            if (arranjo == null) return;
            // (Lógica de detalhes igual ao anterior...)
            string detalhes = $"Cliente: {arranjo.Cliente.FirstName} {arranjo.Cliente.LastName}\n";
            detalhes += $"Data: {arranjo.DataEntrada}\n\n";
            detalhes += "ITENS PARA ARRANJAR:\n";

            foreach (var item in arranjo.ListaCalcado)
            {
                detalhes += $"- {item.NumPar} ({item.Tipo} - {item.Cor})\n";
                detalhes += $"  Serviços: {string.Join(", ", item.ServicosParaFazer)}\n";
            }
            MessageBox.Show(detalhes, $"Talão #{arranjo.Id}");
        }
    }
}