using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using POOProject.ViewModels;
using System;

namespace POOProjectTests.ViewModelsTest
{
    [TestClass]
    public class DetalhesTalaoViewModelTests
    {
        [TestMethod]
        public void CarregarDados_DeveFormatarTextoCorretamente()
        {
            // 1. Arrange
            var vm = new DetalhesTalaoViewModel();

            var cliente = new Cliente("Maria", "Santos");
            var funcionario = new Funcionario("Joao", "Silva", 1);
            var arranjo = new Arranjo(cliente, funcionario);

            // Vamos forçar uma data específica para o teste não falhar por causa dos segundos
            arranjo.DataEntrada = new DateTime(2023, 12, 25, 14, 30, 0); // Natal às 14:30

            var sapato = new Calcado { NumPar = "Par 1", Tipo = TipoCalcado.Bota, Cor = CorCalcado.Castanho };
            sapato.ServicosParaFazer.Add(Servicos.Colar);
            sapato.ServicosParaFazer.Add(Servicos.Pintar); // Adicionar 2 serviços para testar a vírgula
            arranjo.AdicionarCalcado(sapato);

            // 2. Act
            vm.CarregarDados(arranjo);

            // 3. Assert
            // Verifica se o ID apareceu no título
            Assert.IsTrue(vm.TituloTalao.Contains(arranjo.Id), "O título deve conter o ID do talão.");

            // Verifica a formatação da data (dd/MM/yyyy HH:mm)
            Assert.IsTrue(vm.InfoData.Contains("25/12/2023 14:30"), "A data não está formatada corretamente.");

            // Verifica se a lista de serviços ficou separada por vírgula
            string textoServicos = vm.ItensParaMostrar[0].ListaServicos;
            Assert.IsTrue(textoServicos.Contains("Colar, Pintar"), $"A lista de serviços falhou. Resultado: {textoServicos}");
        }
    }
}