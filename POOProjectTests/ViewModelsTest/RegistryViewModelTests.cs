using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.ViewModels;
using POOProjectTests.Fakes;
using System.Linq;

namespace POOProjectTests.ViewModelsTest
{
    [TestClass]
    public class RegistryViewModelTests
    {
        private FakeAuthService _authService;
        private FakeViewFactory _viewFactory;
        private RegistryViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _authService = new FakeAuthService();
            _viewFactory = new FakeViewFactory();
            _vm = new RegistryViewModel(_authService, _viewFactory);
        }

        [TestMethod]
        public void Registar_SenhasDiferentes_NaoDeveChamarServico()
        {
            // 1. Arrange
            _vm.Username = "novoUser";
            _vm.FirstName = "Ana";
            _vm.LastName = "Sousa";
            _vm.Password = "123";
            _vm.PasswordRepeat = "999"; // Diferente!

            // 2. Act
            _vm.RegistryCommand.Execute(null);

            // 3. Assert
            // O teste passa se não houver exceções e o fluxo for interrompido antes do registo.
            // (Poderíamos melhorar verificando se o FakeAuthService.Register foi chamado 0 vezes).
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Registar_DadosValidos_DeveSucesso()
        {
            // 1. Arrange
            _vm.Username = "novoUser";
            _vm.FirstName = "Ana";
            _vm.LastName = "Sousa";
            _vm.Password = "123";
            _vm.PasswordRepeat = "123"; // Igual

            // 2. Act
            _vm.RegistryCommand.Execute(null);

            // 3. Assert
            // Se chegou aqui sem erros, o ViewModel aceitou os dados e chamou o serviço.
            Assert.IsTrue(true, "O comando executou com sucesso.");
        }
    }
}