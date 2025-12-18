using POOProject.Models.Enums;
using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace POOProject.ViewModels
{
    public class AddArranjoViewModel : BaseViewModel
    {
        private readonly IArranjoRepository _repository;
        private readonly IFuncionarioRepository _funcionarioRepository;

        // --- NOVO: Ação para mostrar mensagens (Testável!) ---
        // Por defeito, usa a MessageBox real. Nos testes, vamos mudar isto.
        public Action<string> ShowMessageAction { get; set; } = (msg) => MessageBox.Show(msg);

        // Dados do Formulário
        private string _nomeCliente = string.Empty;
        private string _sobrenomeCliente = string.Empty;
        private int _numeroPares = 1;

        // --- LISTAS E SELEÇÕES ---
        public ObservableCollection<RepairItemViewModel> RepairItems { get; set; }
        public ObservableCollection<Funcionario> ListaFuncionarios { get; set; }

        private Funcionario? _funcionarioSelecionado;
        public Funcionario? FuncionarioSelecionado
        {
            get => _funcionarioSelecionado;
            set { _funcionarioSelecionado = value; OnPropertyChanged(nameof(FuncionarioSelecionado)); }
        }

        public ICommand SaveCommand { get; }
        public Action? HideWindowAction { get; set; }

        public string NomeCliente
        {
            get => _nomeCliente;
            set { _nomeCliente = value; OnPropertyChanged(nameof(NomeCliente)); }
        }

        public string SobrenomeCliente
        {
            get => _sobrenomeCliente;
            set { _sobrenomeCliente = value; OnPropertyChanged(nameof(SobrenomeCliente)); }
        }

        public int NumeroPares
        {
            get => _numeroPares;
            set
            {
                _numeroPares = value;
                OnPropertyChanged(nameof(NumeroPares));
                UpdateList();
            }
        }

        public AddArranjoViewModel(IArranjoRepository repository, IFuncionarioRepository funcionarioRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));

            RepairItems = new ObservableCollection<RepairItemViewModel>();
            ListaFuncionarios = new ObservableCollection<Funcionario>();

            SaveCommand = new ViewModelCommand(ExecuteSave);

            UpdateList();
            CarregarFuncionarios();
        }

        private void CarregarFuncionarios()
        {
            ListaFuncionarios.Clear();
            var todos = _funcionarioRepository.GetAll();
            foreach (var f in todos) ListaFuncionarios.Add(f);

            if (ListaFuncionarios.Count > 0) FuncionarioSelecionado = ListaFuncionarios[0];
        }

        private void UpdateList()
        {
            RepairItems.Clear();
            var todosServicos = Enum.GetValues(typeof(Servicos)).Cast<Servicos>().ToList();

            for (int i = 1; i <= NumeroPares; i++)
            {
                var itemVM = new RepairItemViewModel { Title = $"Par {i}" };
                foreach (var servicoEnum in todosServicos)
                {
                    itemVM.AvailableServices.Add(new ServiceOptionViewModel
                    {
                        Name = servicoEnum.ToString(),
                        EnumValue = servicoEnum,
                        IsSelected = false
                    });
                }
                RepairItems.Add(itemVM);
            }
        }

        private void ExecuteSave(object? obj)
        {
            if (string.IsNullOrWhiteSpace(NomeCliente))
            {
                ShowMessageAction("Preencha o nome do cliente."); // USAR NOVA AÇÃO
                return;
            }
            if (FuncionarioSelecionado == null)
            {
                ShowMessageAction("Selecione o Funcionário Responsável."); // USAR NOVA AÇÃO
                return;
            }

            foreach (var item in RepairItems)
            {
                bool temServico = item.AvailableServices.Any(s => s.IsSelected);
                if (!temServico)
                {
                    // USAR NOVA AÇÃO
                    ShowMessageAction($"O '{item.Title}' não tem nenhum serviço selecionado.\nSelecione pelo menos um serviço.");
                    return;
                }
            }

            try
            {
                var cliente = new Cliente(NomeCliente, SobrenomeCliente);
                var novoArranjo = new Arranjo(cliente, FuncionarioSelecionado);

                foreach (var vm in RepairItems)
                {
                    var calcado = new Calcado
                    {
                        NumPar = vm.Title,
                        Tipo = vm.SelectedTipo,
                        Cor = vm.SelectedCor,
                        Descricao = vm.Description
                    };

                    foreach (var opcao in vm.AvailableServices)
                    {
                        if (opcao.IsSelected) calcado.ServicosParaFazer.Add(opcao.EnumValue);
                    }
                    novoArranjo.AdicionarCalcado(calcado);
                }

                _repository.SaveArranjo(novoArranjo);

                ShowMessageAction($"Talão {novoArranjo.Id} criado com sucesso!"); // USAR NOVA AÇÃO
                HideWindowAction?.Invoke();
            }
            catch (Exception ex)
            {
                ShowMessageAction($"Erro: {ex.Message}"); // USAR NOVA AÇÃO
            }
        }
    }
}