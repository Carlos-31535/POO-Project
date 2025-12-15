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

        // Dados do Formulário
        private string _nomeCliente = string.Empty;
        private string _sobrenomeCliente = string.Empty;
        private int _numeroPares = 1;

        // Lista que aparece no ecrã
        public ObservableCollection<RepairItemViewModel> RepairItems { get; set; }

        public ICommand SaveCommand { get; }

        // Ação para fechar a janela (o ViewFactory preenche isto)
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
                UpdateList(); // Recria a lista quando mudas o número
            }
        }

        // Construtor
        public AddArranjoViewModel(IArranjoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            RepairItems = new ObservableCollection<RepairItemViewModel>();
            SaveCommand = new ViewModelCommand(ExecuteSave);

            // Inicializa a lista com 1 par
            UpdateList();
        }

        // Gera as "Fichas" para cada par de sapatos
        private void UpdateList()
        {
            RepairItems.Clear();

            // Pega em todos os serviços do Enum "Servicos"
            var todosServicos = Enum.GetValues(typeof(Servicos)).Cast<Servicos>().ToList();

            for (int i = 1; i <= NumeroPares; i++)
            {
                var itemVM = new RepairItemViewModel { Title = $"Par {i}" };

                // Para cada par, cria as checkboxes de serviços
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

        // Botão Guardar
        private void ExecuteSave(object? obj)
        {
            if (string.IsNullOrWhiteSpace(NomeCliente))
            {
                MessageBox.Show("Preencha o nome do cliente.");
                return;
            }

            // 1. Criar o Objeto ARRANJO (Entidade do Prof)
            var novoArranjo = new Arranjo
            {
                Cliente = new Cliente(NomeCliente, SobrenomeCliente),
                FuncionarioResponsavel = new Funcionario("Admin", "User", 1), // Trocar para marcar o user que esta a logado
                Estado = EstadoArranjo.Arranjar,
                DataEntrada = DateTime.Now
            };

            // 2. Converter os dados do ecrã para a Lista de Calçado
            foreach (var vm in RepairItems)
            {
                var calcado = new Calcado
                {
                    NumPar = vm.Title,
                    Tipo = vm.SelectedTipo, // Vem da ComboBox
                    Cor = vm.SelectedCor,   // Vem da ComboBox
                    Descricao = vm.Description
                };

                // Verificar quais serviços foram marcados
                foreach (var opcao in vm.AvailableServices)
                {
                    if (opcao.IsSelected)
                    {
                        calcado.ServicosParaFazer.Add(opcao.EnumValue);
                    }
                }

                novoArranjo.ListaCalcado.Add(calcado);
            }

            // 3. Guardar no Repositório
            _repository.SaveArranjo(novoArranjo);

            MessageBox.Show($"Talão {novoArranjo.Id} criado com sucesso!");

            // Fecha a janela
            HideWindowAction?.Invoke();
        }
    }
}
