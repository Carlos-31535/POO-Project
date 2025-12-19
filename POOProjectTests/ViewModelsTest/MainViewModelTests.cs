using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using POOProject.ViewModels;
using POOProjectTests.Fakes;
using System.Linq;

namespace POOProjectTests.ViewModelsTest
{
    [TestClass]
    public class MainViewModelTests
    {
        private FakeArranjoRepo _arranjoRepo;
        private FakeFuncionarioRepo _funcRepo;
        private FakeViewFactory _viewFactory;
        private MainViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            // As classes Fake agora vêm dos ficheiros separados que criaste
            _arranjoRepo = new FakeArranjoRepo();
            _funcRepo = new FakeFuncionarioRepo();
            _viewFactory = new FakeViewFactory();

            var clienteA = new Cliente("Ana", "Silva");
            var clienteB = new Cliente("Bruno", "Costa");
            var func = new Funcionario("Pedro", "Santos", 1);

            var arranjoPendente = new Arranjo(clienteA, func);
            var arranjoPronto = new Arranjo(clienteB, func);

            // Adicionar sapatos para validar regras
            arranjoPronto.AdicionarCalcado(new Calcado { NumPar = "P1", ServicosParaFazer = { Servicos.Colar } });
            arranjoPronto.MarcarComoPronto();

            _arranjoRepo.SaveArranjo(arranjoPendente);
            _arranjoRepo.SaveArranjo(arranjoPronto);

            _viewModel = new MainViewModel(_viewFactory, _arranjoRepo, _funcRepo);
            _viewModel.MessageBoxAction = (msg) => { };
        }
        [TestMethod]
        public void Pesquisa_DeveSerInsensivelAMaiusculas_E_ResetarQuandoVazia()
        {
            // 1. Arrange - Vamos para a lista de pendentes
            _viewModel.ShowPendingArranjoCommand.Execute(null);
            int totalPendentes = _viewModel.Arranjos.Count;

            // 2. Act & Assert - Pesquisa "ANA" (Maiúsculas) quando o nome é "Ana"
            _viewModel.SearchText = "ANA";
            Assert.AreEqual(1, _viewModel.Arranjos.Count, "Devia encontrar 'Ana' mesmo escrevendo 'ANA'.");

            // 3. Act & Assert - Pesquisa Parcial "ilva" (de Silva)
            _viewModel.SearchText = "ilva";
            Assert.AreEqual(1, _viewModel.Arranjos.Count, "Devia encontrar 'Silva' pesquisando 'ilva'.");

            // 4. Act & Assert - Limpar a pesquisa
            _viewModel.SearchText = "";
            Assert.AreEqual(totalPendentes, _viewModel.Arranjos.Count, "Ao limpar a pesquisa, a lista deve voltar ao total inicial.");
        }

        [TestMethod]
        public void NavegarParaPendentes_DeveMostrarApenasPendentes()
        {
            _viewModel.ShowPendingArranjoCommand.Execute(null);
            Assert.AreEqual(1, _viewModel.Arranjos.Count);
            Assert.AreEqual("Ana Silva", _viewModel.Arranjos[0].Cliente.NomeCompleto);
        }

        [TestMethod]
        public void NavegarParaProntos_DeveMostrarApenasProntos()
        {
            _viewModel.ShowFinishedArranjoCommand.Execute(null);
            Assert.AreEqual(1, _viewModel.Arranjos.Count);
            Assert.AreEqual("Bruno Costa", _viewModel.Arranjos[0].Cliente.NomeCompleto);
        }

        [TestMethod]
        public void PesquisarArranjo_PorNome_DeveFiltrarLista()
        {
            _viewModel.ShowPendingArranjoCommand.Execute(null);

            _viewModel.SearchText = "Ana";
            Assert.AreEqual(1, _viewModel.Arranjos.Count);

            _viewModel.SearchText = "Xavier";
            Assert.AreEqual(0, _viewModel.Arranjos.Count);
        }

        [TestMethod]
        public void MarcarComoPronto_DeveAtualizarRepoERemoverDaListaPendente()
        {
            _viewModel.ShowPendingArranjoCommand.Execute(null);
            var arranjoParaConcluir = _viewModel.Arranjos.First();

            // Adiciona serviço para poder concluir
            arranjoParaConcluir.AdicionarCalcado(new Calcado { NumPar = "S1", ServicosParaFazer = { Servicos.Pintar } });

            _viewModel.MarkAsReadyCommand.Execute(arranjoParaConcluir);

            Assert.AreEqual(0, _viewModel.Arranjos.Count);
            var arranjoNaBD = _arranjoRepo.GetAllArranjos().First(a => a.Cliente.FirstName == "Ana");
            Assert.AreEqual(EstadoArranjo.Pronto, arranjoNaBD.Estado);
        }
    }
}