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
    // Representa UMA Checkbox de serviço (Ex: [x] Colar)
    public class ServiceOptionViewModel : BaseViewModel
    {
        private bool _isSelected;
        public string Name { get; set; } = "";    // Texto para o ecrã
        public Servicos EnumValue { get; set; }   // Valor real do Enum

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }
    }

    // Representa UM Par de Sapatos no ecrã (com as suas comboboxes e checkboxes)
    public class RepairItemViewModel : BaseViewModel
    {
        public string Title { get; set; } = "";      // "Par 1"
        public string Description { get; set; } = ""; // "Sola partida"

        // Dados selecionados nas ComboBoxes
        public TipoCalcado SelectedTipo { get; set; }
        public CorCalcado SelectedCor { get; set; }

        // Listas para encher as ComboBoxes (Binding)
        public IEnumerable<TipoCalcado> TiposDisponiveis => Enum.GetValues(typeof(TipoCalcado)).Cast<TipoCalcado>();
        public IEnumerable<CorCalcado> CoresDisponiveis => Enum.GetValues(typeof(CorCalcado)).Cast<CorCalcado>();

        // Lista de Checkboxes (Colar, Coser, etc.)
        public ObservableCollection<ServiceOptionViewModel> AvailableServices { get; set; }
            = new ObservableCollection<ServiceOptionViewModel>();
    }
}
