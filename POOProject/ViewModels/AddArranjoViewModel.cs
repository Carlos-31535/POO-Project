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

        // Abstração da MessageBox para permitir testes unitários (unit testing)
        public Action<string> ShowMessageAction { get; set; } = (msg) => MessageBox.Show(msg);

        private string _nomeCliente = string.Empty;
        private string _sobrenomeCliente = string.Empty;
        private int _numeroPares = 1;

        // Lista dinâmica que cresce conforme o número de pares escolhido
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
                // Recria a lista de formulários sempre que mudamos a quantidade
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

            // Pré-seleciona o primeiro para agilizar
            if (ListaFuncionarios.Count > 0) FuncionarioSelecionado = ListaFuncionarios[0];
        }

        private void UpdateList()
        {
            RepairItems.Clear();
            var todosServicos = Enum.GetValues(typeof(Servicos)).Cast<Servicos>().ToList();

            // Cria N formulários de sapatos (Par 1, Par 2...)
            for (int i = 1; i <= NumeroPares; i++)
            {
                var itemVM = new RepairItemViewModel { Title = $"Par {i}" };

                // Preenche as checkboxes de serviços para cada par
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
            // Validações de entrada
            if (string.IsNullOrWhiteSpace(NomeCliente))
            {
                ShowMessageAction("Preencha o nome do cliente.");
                return;
            }
            if (FuncionarioSelecionado == null)
            {
                ShowMessageAction("Selecione o Funcionário Responsável.");
                return;
            }

            // Garante que cada sapato tem pelo menos um serviço marcado
            foreach (var item in RepairItems)
            {
                bool temServico = item.AvailableServices.Any(s => s.IsSelected);
                if (!temServico)
                {
                    ShowMessageAction($"O '{item.Title}' não tem nenhum serviço selecionado.\nSelecione pelo menos um serviço.");
                    return;
                }
            }

            try
            {
                // Conversão: ViewModels -> Entidades de Domínio
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

                    // Mapear apenas os serviços que foram marcados com 'Check'
                    foreach (var opcao in vm.AvailableServices)
                    {
                        if (opcao.IsSelected) calcado.ServicosParaFazer.Add(opcao.EnumValue);
                    }
                    novoArranjo.AdicionarCalcado(calcado);
                }

                _repository.SaveArranjo(novoArranjo);

                ShowMessageAction($"Talão {novoArranjo.Id} criado com sucesso!");
                HideWindowAction?.Invoke();
            }
            catch (Exception ex)
            {
                ShowMessageAction($"Erro: {ex.Message}");
            }
        }
    }
}