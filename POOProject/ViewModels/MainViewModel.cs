using System;
using System.Collections.ObjectModel;
using System.Linq; // Necessário para filtrar (Where)
using System.Windows;
using System.Windows.Input;
using POOProject.Models.Entities;
using POOProject.Models.Enums; // Confirma o nome da pasta (Enums ou Emuns)
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Commands;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProject.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IViewFactory _viewFactory;
        private readonly IArranjoRepository _arranjoRepository; // <--- NOVO REPOSITÓRIO

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
        public ObservableCollection<Arranjo> Arranjos { get; set; } // <--- NOVA LISTA

        public ICommand OpenArranjoCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ShowPendingArranjoCommand { get; }
        public ICommand ShowFinishedArranjoCommand { get; }
        
        public ICommand ShowFuncionariosCommand { get; }
        public ICommand MarkAsReadyCommand { get; }
        public ICommand ShowDetailsArranjoCommand { get; }

        public MainViewModel(IViewFactory viewFactory, IArranjoRepository arranjoRepository)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _arranjoRepository = arranjoRepository ?? throw new ArgumentNullException(nameof(arranjoRepository));

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

            // Carregar dados iniciais (Funcionários)
            CarregarFuncionariosTeste();
        }

        // --- LÓGICA DE TROCA DE ECRA ---
        private void ExecuteShowPendingArranjos(object? obj)
        {
            IsFuncionariosVisible = false;
            IsArranjosVisible = true;
            TituloTabela = "Arranjos Pendentes (Por Arranjar)";

            // CORREÇÃO AQUI: Usa o nome exato da tua interface
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

            // CORREÇÃO AQUI TAMBÉM:
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
        }

        // --- OUTROS MÉTODOS ---
        private void ExecuteOpenArranjo(object? parameter)
        {
            Window window = _viewFactory.ShowDialog(ViewType.AddArranjo);
            window.ShowDialog();
            // Opcional: Atualizar a lista depois de fechar a janela
            if (IsArranjosVisible) ExecuteShowPendingArranjos(null);
        }

        private void EditEmployee(Funcionario funcionario)
        {
            MessageBox.Show($"A editar: {funcionario.FirstName}");
        }

        //teste basico, eliminar dps
        private void CarregarFuncionariosTeste()
        {
            Funcionarios.Add(new Funcionario("João", "Silva", 1));
            Funcionarios.Add(new Funcionario("Maria", "Santos", 2));
        }


        private void ExecuteMarkAsReady(Arranjo arranjo)
        {
            if (arranjo != null)
            {
                // 1. MUDAR A ETIQUETA
                // Ao fazeres isto, ele já "pertence" à outra página na memória.
                arranjo.Estado = EstadoArranjo.Pronto;

                // 2. AVISAR
                _arranjoRepository.Update(arranjo);
                MessageBox.Show($"Talão {arranjo.Id} marcado como PRONTO! Vai mover-se para a lista de Prontos.", "Sucesso");

                // 3. FAZER ELE "IR" PARA A OUTRA PÁGINA
                // Para ele ir, tem de sair desta. 
                // Isto NÃO APAGA O DADO, só o tira desta lista visual "Por Arranjar".
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
    }
}