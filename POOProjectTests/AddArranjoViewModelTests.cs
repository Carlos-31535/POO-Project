using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using POOProject.ViewModels;
using System.Linq;

namespace POOProjectTests
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
        }

        [TestMethod]
        public void GravarArranjo_SemServicosSelecionados_DeveMostrarErroENaoGravar()
        {
            var vm = new AddArranjoViewModel(_arranjoRepo, _funcRepo);
            vm.NomeCliente = "Cliente Teste";
            vm.SobrenomeCliente = "Teste";

            string mensagemErro = "";
            vm.ShowMessageAction = (msg) => mensagemErro = msg;

            vm.SaveCommand.Execute(null);

            Assert.IsTrue(mensagemErro.Contains("não tem nenhum serviço"));
            Assert.AreEqual(0, _arranjoRepo.ArranjosGravados.Count);
        }

        [TestMethod]
        public void GravarArranjo_TudoValido_DeveGravarComSucesso()
        {
            var vm = new AddArranjoViewModel(_arranjoRepo, _funcRepo);
            vm.NomeCliente = "Sucesso";
            vm.SobrenomeCliente = "Teste";

            string msg = "";
            vm.ShowMessageAction = (m) => msg = m;

            // Selecionar serviço válido (Colar)
            var item = vm.RepairItems[0];
            var servico = item.AvailableServices.First(s => s.EnumValue == Servicos.Colar);
            servico.IsSelected = true;

            vm.SaveCommand.Execute(null);

            Assert.IsTrue(msg.ToLower().Contains("sucesso"), $"Falhou: {msg}");
            Assert.AreEqual(1, _arranjoRepo.ArranjosGravados.Count);
        }
    }
}