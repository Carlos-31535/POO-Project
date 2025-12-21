using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using POOProject.ViewModels;
using POOProjectTests.Fakes;
using System.Linq;

namespace POOProjectTests.ViewModelsTest
{
    [TestClass]
    public class AddArranjoViewModelTests
    {
        private FakeArranjoRepo _arranjoRepo;
        private FakeFuncionarioRepo _funcRepo;

        [TestInitialize]
        public void Setup()
        {
            _arranjoRepo = new FakeArranjoRepo();
            _funcRepo = new FakeFuncionarioRepo();

            // --- TRUQUE DE TESTE ---
            // Numa aplicação real, a ComboBox de funcionários carrega dados da BD.
            // Aqui, se não criarmos um funcionário falso antes, a lista fica vazia,
            // o ViewModel dá erro de "Selecione Funcionário" e o teste falha injustamente.
            var funcionarioTeste = new Funcionario("Funcionario", "Teste", 1);
            _funcRepo.SaveOrUpdate(funcionarioTeste);
        }

        [TestMethod]
        public void GravarArranjo_SemServicosSelecionados_DeveMostrarErroENaoGravar()
        {
            // 1. Arrange
            var vm = new AddArranjoViewModel(_arranjoRepo, _funcRepo);
            vm.NomeCliente = "Cliente Teste";
            vm.SobrenomeCliente = "Teste";
            // (O funcionário é selecionado automaticamente no construtor porque já existe no repo)

            // Intercepta a MessageBox para não bloquear o teste e validar a mensagem
            string mensagemErro = "";
            vm.ShowMessageAction = (msg) => mensagemErro = msg;

            // 2. Act
            vm.SaveCommand.Execute(null);

            // 3. Assert
            // Verifica se a UI avisou o utilizador
            Assert.IsTrue(mensagemErro.Contains("não tem nenhum serviço"), $"Mensagem inesperada: {mensagemErro}");

            // Verifica se a base de dados continua vazia (garantia de integridade)
            Assert.AreEqual(0, _arranjoRepo.ArranjosGravados.Count);
        }

        [TestMethod]
        public void GravarArranjo_TudoValido_DeveGravarComSucesso()
        {
            // 1. Arrange
            var vm = new AddArranjoViewModel(_arranjoRepo, _funcRepo);
            vm.NomeCliente = "Sucesso";
            vm.SobrenomeCliente = "Teste";

            // Fallback de segurança para seleção de funcionário
            if (vm.FuncionarioSelecionado == null)
                vm.FuncionarioSelecionado = vm.ListaFuncionarios.FirstOrDefault();

            string msg = "";
            vm.ShowMessageAction = (m) => msg = m;

            // Simula o clique do utilizador na Checkbox "Colar" do primeiro par
            var item = vm.RepairItems[0];
            var servico = item.AvailableServices.First(s => s.EnumValue == Servicos.Colar);
            servico.IsSelected = true;

            // 2. Act
            vm.SaveCommand.Execute(null);

            // 3. Assert
            Assert.IsTrue(msg.ToLower().Contains("sucesso"), "A mensagem de sucesso não apareceu.");

            // Validação final: O repositório recebeu o objeto?
            Assert.AreEqual(1, _arranjoRepo.ArranjosGravados.Count);
        }
    }
}