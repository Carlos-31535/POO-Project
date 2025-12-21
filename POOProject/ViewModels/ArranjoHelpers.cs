using POOProject.Models.Enums;
using POOProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.ViewModels
{
    // ViewModel auxiliar que representa uma opção de Checkbox na interface.
    // Permite fazer Binding do estado "IsSelected" diretamente para o WPF.
    public class ServiceOptionViewModel : BaseViewModel
    {
        private bool _isSelected;
        public string Name { get; set; } = "";
        public Servicos EnumValue { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }
    }

    // ViewModel auxiliar para representar cada par de sapatos na lista de criação.
    // Agrupa as características (Cor, Tipo) e os serviços a realizar nesse par.
    public class RepairItemViewModel : BaseViewModel
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public TipoCalcado SelectedTipo { get; set; }
        public CorCalcado SelectedCor { get; set; }

        // Fontes de dados para preencher as ComboBoxes (converte Enums para lista)
        public IEnumerable<TipoCalcado> TiposDisponiveis => Enum.GetValues(typeof(TipoCalcado)).Cast<TipoCalcado>();
        public IEnumerable<CorCalcado> CoresDisponiveis => Enum.GetValues(typeof(CorCalcado)).Cast<CorCalcado>();

        public ObservableCollection<ServiceOptionViewModel> AvailableServices { get; set; }
            = new ObservableCollection<ServiceOptionViewModel>();
    }
}