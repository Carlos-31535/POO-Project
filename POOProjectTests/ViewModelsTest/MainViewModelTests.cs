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
        // Instâncias falsas para controlar o teste
        private FakeArranjoRepo _arranjoRepo;
        private FakeFuncionarioRepo _funcRepo;
        private FakeViewFactory _viewFactory;
        private MainViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _arranjoRepo = new FakeArranjoRepo();
            _funcRepo = new FakeFuncionarioRepo();
            _viewFactory = new FakeViewFactory();

            // --- SEEDING (Sementeira de Dados) ---
            // Injetamos dados iniciais para ter o que testar nas listas
            var clienteA = new Cliente("Ana", "Silva");
            var clienteB = new Cliente("Bruno", "Costa");
            var func = new Funcionario("Pedro", "Santos", 1);

            var arranjoPendente = new Arranjo(clienteA, func);
            var arranjoPronto = new Arranjo(clienteB, func);

            // Preparamos um arranjo pronto para teste de filtragem
            arranjoPronto.AdicionarCalcado(new Calcado { NumPar = "P1", ServicosParaFazer = { Servicos.Colar } });
            arranjoPronto.MarcarComoPronto();

            _arranjoRepo.SaveArranjo(arranjoPendente);
            _arranjoRepo.SaveArranjo(arranjoPronto);

            // Inicializa o ViewModel com os repositórios já cheios
            _viewModel = new MainViewModel(_viewFactory, _arranjoRepo, _funcRepo);
            _viewModel.MessageBoxAction = (msg) => { }; // Silencia mensagens
        }

        [TestMethod]
        public void Pesquisa_DeveSerInsensivelAMaiusculas_E_ResetarQuandoVazia()
        {
            // 1. Carrega lista de Pendentes
            _viewModel.ShowPendingArranjoCommand.Execute(null);
            int totalPendentes = _viewModel.Arranjos.Count;

            // 2. Teste Case Insensitive ("ANA" == "Ana")
            _viewModel.SearchText = "ANA";
            Assert.AreEqual(1, _viewModel.Arranjos.Count, "Devia encontrar 'Ana' mesmo escrevendo 'ANA'.");

            // 3. Teste Pesquisa Parcial ("ilva" em "Silva")
            _viewModel.SearchText = "ilva";
            Assert.AreEqual(1, _viewModel.Arranjos.Count, "Devia encontrar 'Silva' pesquisando 'ilva'.");

            // 4. Teste Reset (Apagar pesquisa restaura lista)
            _viewModel.SearchText = "";
            Assert.AreEqual(totalPendentes, _viewModel.Arranjos.Count, "Ao limpar a pesquisa, a lista deve voltar ao total inicial.");
        }

        [TestMethod]
        public void NavegarParaPendentes_DeveMostrarApenasPendentes()
        {
            // Executa o comando do menu
            _viewModel.ShowPendingArranjoCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.Arranjos.Count, "Devia haver apenas 1 pendente.");
            Assert.AreEqual("Ana Silva", _viewModel.Arranjos[0].Cliente.NomeCompleto);
        }

        [TestMethod]
        public void NavegarParaProntos_DeveMostrarApenasProntos()
        {
            _viewModel.ShowFinishedArranjoCommand.Execute(null);

            Assert.AreEqual(1, _viewModel.Arranjos.Count, "Devia haver apenas 1 pronto.");
            Assert.AreEqual("Bruno Costa", _viewModel.Arranjos[0].Cliente.NomeCompleto);
        }

        [TestMethod]
        public void MarcarComoPronto_DeveAtualizarRepoERemoverDaListaPendente()
        {
            // 1. Vai para a lista de pendentes
            _viewModel.ShowPendingArranjoCommand.Execute(null);
            var arranjoParaConcluir = _viewModel.Arranjos.First();

            // Adiciona serviço à força para permitir concluir (evita erro de validação)
            arranjoParaConcluir.AdicionarCalcado(new Calcado { NumPar = "S1", ServicosParaFazer = { Servicos.Pintar } });

            // 2. Executa comando "Marcar Pronto"
            _viewModel.MarkAsReadyCommand.Execute(arranjoParaConcluir);

            // 3. Assert
            // Deve desaparecer da lista visual atual (UI Feedback)
            Assert.AreEqual(0, _viewModel.Arranjos.Count, "O arranjo devia ter saído da lista de pendentes.");

            // Deve estar atualizado na 'Base de Dados'
            var arranjoNaBD = _arranjoRepo.GetAllArranjos().First(a => a.Cliente.FirstName == "Ana");
            Assert.AreEqual(EstadoArranjo.Pronto, arranjoNaBD.Estado, "O estado na BD devia ser 'Pronto'.");
        }
    }
}