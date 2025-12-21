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

            var arranjo = new Arranjo(new Cliente("Maria", "Santos"), new Funcionario("Joao", "Silva", 1));

            // Fixamos a data para o teste ser determinístico (não falhar dependendo da hora do dia)
            arranjo.DataEntrada = new DateTime(2023, 12, 25, 14, 30, 0);

            var sapato = new Calcado { NumPar = "Par 1", Tipo = TipoCalcado.Bota, Cor = CorCalcado.Castanho };
            sapato.ServicosParaFazer.Add(Servicos.Colar);
            sapato.ServicosParaFazer.Add(Servicos.Pintar);
            arranjo.AdicionarCalcado(sapato);

            // 2. Act - O ViewModel processa o objeto 'Arranjo'
            vm.CarregarDados(arranjo);

            // 3. Assert - Validamos o que aparece no ecrã

            // Título dinâmico
            Assert.IsTrue(vm.TituloTalao.Contains(arranjo.Id), "O título deve conter o ID do talão.");

            // Formatação de Data
            Assert.IsTrue(vm.InfoData.Contains("25/12/2023 14:30"), "A data não está formatada corretamente.");

            // Concatenação de lista (String.Join)
            string textoServicos = vm.ItensParaMostrar[0].ListaServicos;
            Assert.IsTrue(textoServicos.Contains("Colar, Pintar"), $"A lista de serviços falhou. Resultado: {textoServicos}");
        }
    }
}