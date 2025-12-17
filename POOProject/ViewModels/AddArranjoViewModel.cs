using POOProject.Models.Enums;
using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POOProject.ViewModels
{
    public class AddArranjoViewModel : BaseViewModel
    {
        private readonly IArranjoRepository _repository;
        private readonly IFuncionarioRepository _funcionarioRepository; // <--- 1. NOVO: Repositório adicionado

        // Dados do Formulário
        private string _nomeCliente = string.Empty;
        private string _sobrenomeCliente = string.Empty;
        private int _numeroPares = 1;

        // --- LISTAS E SELEÇÕES ---
        public ObservableCollection<RepairItemViewModel> RepairItems { get; set; }

        // 2. NOVA LISTA para a ComboBox
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
        // 3. Injetamos aqui o IFuncionarioRepository
        public AddArranjoViewModel(IArranjoRepository repository, IFuncionarioRepository funcionarioRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));

            RepairItems = new ObservableCollection<RepairItemViewModel>();
            ListaFuncionarios = new ObservableCollection<Funcionario>(); // Inicializa a lista

            SaveCommand = new ViewModelCommand(ExecuteSave);

            // Inicializa a lista de sapatos
            UpdateList();

            // 4. Carrega os funcionários para a ComboBox
            CarregarFuncionarios();
        }

        private void CarregarFuncionarios()
        {
            ListaFuncionarios.Clear();

            // Vai buscar todos ao ficheiro JSON
            var todos = _funcionarioRepository.GetAll();

            foreach (var f in todos)
            {
                ListaFuncionarios.Add(f);
            }

            // Seleciona o primeiro automaticamente (opcional, para não vir vazio)
            if (ListaFuncionarios.Count > 0)
            {
                FuncionarioSelecionado = ListaFuncionarios[0];
            }
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
                MessageBox.Show("Preencha o nome do cliente.");
                return;
            }

            // 5. Validação: Obrigatório escolher funcionário
            if (FuncionarioSelecionado == null)
            {
                MessageBox.Show("Selecione o Funcionário Responsável.");
                return;
            }

            var novoArranjo = new Arranjo
            {
                Cliente = new Cliente(NomeCliente, SobrenomeCliente),

                // 6. Usa o funcionário selecionado na ComboBox em vez do "Admin" falso
                FuncionarioResponsavel = FuncionarioSelecionado,

                Estado = EstadoArranjo.Arranjar,
                DataEntrada = DateTime.Now
            };

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

            _repository.SaveArranjo(novoArranjo);
            MessageBox.Show($"Talão {novoArranjo.Id} criado com sucesso!");
            HideWindowAction?.Invoke();
        }
    }
}