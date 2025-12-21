using POOProject.Models.Entities;
using POOProject.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace POOProject.ViewModels
{
    public class DetalhesTalaoViewModel : BaseViewModel
    {
        private string _tituloTalao = string.Empty;
        private string _infoCliente = string.Empty;
        private string _infoData = string.Empty;
        private string _infoFuncionario = string.Empty;

        public string TituloTalao
        {
            get => _tituloTalao;
            set { _tituloTalao = value; OnPropertyChanged(nameof(TituloTalao)); }
        }

        public string InfoCliente
        {
            get => _infoCliente;
            set { _infoCliente = value; OnPropertyChanged(nameof(InfoCliente)); }
        }

        public string InfoData
        {
            get => _infoData;
            set { _infoData = value; OnPropertyChanged(nameof(InfoData)); }
        }

        public string InfoFuncionario
        {
            get => _infoFuncionario;
            set { _infoFuncionario = value; OnPropertyChanged(nameof(InfoFuncionario)); }
        }

        public ObservableCollection<ItemDisplay> ItensParaMostrar { get; set; }

        public ICommand FecharCommand { get; }
        public Action? HideWindowAction { get; set; }

        public DetalhesTalaoViewModel()
        {
            ItensParaMostrar = new ObservableCollection<ItemDisplay>();
            FecharCommand = new RelayCommand<object>(_ => HideWindowAction?.Invoke());
        }

        // Recebe a entidade Arranjo completa e formata os dados para strings simples
        // que a View consegue mostrar sem conversores complexos.
        public void CarregarDados(Arranjo arranjo)
        {
            if (arranjo == null) return;

            TituloTalao = $"TALÃO #{arranjo.Id}";
            InfoCliente = $"Cliente: {arranjo.Cliente?.NomeCompleto ?? "N/A"}";
            InfoData = $"Entrada: {arranjo.DataEntrada:dd/MM/yyyy HH:mm}";
            InfoFuncionario = $"Atendido por: {arranjo.FuncionarioResponsavel?.FirstName}";

            ItensParaMostrar.Clear();
            foreach (var item in arranjo.ListaCalcado)
            {
                ItensParaMostrar.Add(new ItemDisplay
                {
                    Descricao = $"{item.NumPar} - {item.Tipo} ({item.Cor})",
                    // Junta todos os serviços numa string separada por vírgulas
                    ListaServicos = string.Join(", ", item.ServicosParaFazer)
                });
            }
        }
    }

    // Classe DTO (Data Transfer Object) simples apenas para apresentação nesta lista
    public class ItemDisplay
    {
        public string Descricao { get; set; }
        public string ListaServicos { get; set; }
    }
}