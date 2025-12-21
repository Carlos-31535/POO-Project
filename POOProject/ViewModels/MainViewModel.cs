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

        // Action injetável para facilitar testes sem janelas pop-up reais
        public Action<string> MessageBoxAction { get; set; } = (msg) => MessageBox.Show(msg);

        // Controlos de visibilidade para alternar entre a tabela de Funcionários e Arranjos
        private bool _isFuncionariosVisible = true;
        private bool _isArranjosVisible = false;
        private string _tituloTabela = "Lista de Funcionários";

        private string _searchText = string.Empty;

        // Mantém uma cópia local de todos os dados para que a pesquisa seja rápida 
        // sem ter de ir ao disco/base de dados a cada letra escrita.
        private List<Arranjo> _listaCompletaCache = new List<Arranjo>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FiltrarLista(); // Atualiza a grid em tempo real
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

        // Collections observáveis: qualquer Add/Remove reflete-se logo na DataGrid
        public ObservableCollection<Funcionario> Funcionarios { get; set; }
        public ObservableCollection<Arranjo> Arranjos { get; set; }

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

            // RelayCommand<T> permite receber o objeto da linha clicada na tabela
            MarkAsReadyCommand = new RelayCommand<Arranjo>(ExecuteMarkAsReady);
            ShowDetailsArranjoCommand = new RelayCommand<Arranjo>(ExecuteShowDetailsArranjos);
            OpenAddFuncionarioCommand = new ViewModelCommand(ExecuteOpenAddFuncionario);

            CarregarFuncionariosReais();
        }

        private void FiltrarLista()
        {
            if (!IsArranjosVisible) return;

            Arranjos.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Repõe a lista original se a pesquisa estiver vazia
                foreach (var item in _listaCompletaCache) Arranjos.Add(item);
            }
            else
            {
                // Pesquisa insensível a maiúsculas (Case Insensitive) por Cliente ou Nº Talão
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

        private void ExecuteShowPendingArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Pendentes";
            SearchText = "";

            var todos = _arranjoRepository.GetAllArranjos();

            // Filtra: Tudo o que não está pronto nem entregue (trabalho por fazer)
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

        private void ExecuteOpenArranjo(object? parameter)
        {
            // Abre janela modal (bloqueia a principal até fechar)
            Window window = _viewFactory.ShowDialog(ViewType.AddArranjo);
            window.ShowDialog();

            // Atualiza a lista ao voltar, caso tenhamos adicionado algo novo
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
                // Atualiza o estado na memória
                arranjo.MarcarComoPronto();

                // Persiste a alteração no ficheiro JSON
                _arranjoRepository.Update(arranjo);

                MessageBoxAction($"Talão {arranjo.Id} marcado como PRONTO!");

                // Remove da vista atual para dar feedback visual imediato de "Tarefa Concluída"
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

            Window window = _viewFactory.ShowDialog(ViewType.DetalhesTalao);

            // Passamos os dados para o ViewModel da nova janela
            if (window.DataContext is DetalhesTalaoViewModel vm)
            {
                vm.CarregarDados(arranjo);
            }

            window.ShowDialog();
        }
    }
}