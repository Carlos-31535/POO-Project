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

            // --- CORREÇÃO: Adicionar um funcionário à "BD" para o ViewModel carregar ---
            // Sem isto, a ComboBox fica vazia e dá erro "Selecione Funcionário"
            var funcionarioTeste = new Funcionario("Funcionario", "Teste", 1);
            _funcRepo.SaveOrUpdate(funcionarioTeste);
        }

        [TestMethod]
        public void GravarArranjo_SemServicosSelecionados_DeveMostrarErroENaoGravar()
        {
            // O construtor vai carregar o funcionário que criámos no Setup
            var vm = new AddArranjoViewModel(_arranjoRepo, _funcRepo);

            vm.NomeCliente = "Cliente Teste";
            vm.SobrenomeCliente = "Teste";

            // Capturar mensagem de erro
            string mensagemErro = "";
            vm.ShowMessageAction = (msg) => mensagemErro = msg;

            // Act
            vm.SaveCommand.Execute(null);

            // Assert
            Assert.IsTrue(mensagemErro.Contains("não tem nenhum serviço"), $"Mensagem recebida: {mensagemErro}");
            Assert.AreEqual(0, _arranjoRepo.ArranjosGravados.Count);
        }

        [TestMethod]
        public void GravarArranjo_TudoValido_DeveGravarComSucesso()
        {
            var vm = new AddArranjoViewModel(_arranjoRepo, _funcRepo);

            vm.NomeCliente = "Sucesso";
            vm.SobrenomeCliente = "Teste";

            // Garantir que temos funcionário selecionado (caso o load automático falhe)
            if (vm.FuncionarioSelecionado == null)
            {
                vm.FuncionarioSelecionado = vm.ListaFuncionarios.FirstOrDefault();
            }

            string msg = "";
            vm.ShowMessageAction = (m) => msg = m;

            // Selecionar serviço válido (Colar no primeiro par)
            var item = vm.RepairItems[0];
            var servico = item.AvailableServices.First(s => s.EnumValue == Servicos.Colar);
            servico.IsSelected = true;

            // Act
            vm.SaveCommand.Execute(null);

            // Assert
            Assert.IsTrue(msg.ToLower().Contains("sucesso"), $"Falhou: {msg}");
            Assert.AreEqual(1, _arranjoRepo.ArranjosGravados.Count);
        }
    }
}