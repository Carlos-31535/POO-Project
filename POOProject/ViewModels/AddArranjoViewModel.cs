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

        // --- CONSTRUTOR ---
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
            // 1. Validações
            if (string.IsNullOrWhiteSpace(NomeCliente)) { MessageBox.Show("Preencha o nome do cliente."); return; }
            if (FuncionarioSelecionado == null) { MessageBox.Show("Selecione o Funcionário Responsável."); return; }

            // 2. Criar Objeto Arranjo
            var novoArranjo = new Arranjo
            {
                Cliente = new Cliente(NomeCliente, SobrenomeCliente),
                FuncionarioResponsavel = FuncionarioSelecionado,
                Estado = EstadoArranjo.Arranjar,
                DataEntrada = DateTime.Now
            };

            // 3. Adicionar Sapatos
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
                novoArranjo.ListaCalcado.Add(calcado);
            }

            // 4. Guardar na BD
            _repository.SaveArranjo(novoArranjo);

            // 5. Mensagem Simples e Fechar Janela (VOLTOU AO ORIGINAL)
            MessageBox.Show($"Talão {novoArranjo.Id} criado com sucesso!", "Sucesso");
            HideWindowAction?.Invoke();
        }
    }
}