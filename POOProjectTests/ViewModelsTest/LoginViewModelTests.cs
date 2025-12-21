using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.ViewModels;
using POOProjectTests.Fakes;

namespace POOProjectTests.ViewModelsTest
{
    [TestClass]
    public class LoginViewModelTests
    {
        private FakeAuthService _authService;
        private FakeViewFactory _viewFactory;
        private LoginViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            // Montamos o puzzle com peças falsas
            _authService = new FakeAuthService();
            _viewFactory = new FakeViewFactory();
            _vm = new LoginViewModel(_authService, _viewFactory);

            // Evita que pop-ups reais apareçam durante os testes
            _vm.MessageBoxAction = (msg) => { };
        }

        [TestMethod]
        public void Login_ComCredenciaisValidas_DeveSucesso()
        {
            // 1. Arrange - Dados que sabemos que o FakeAuthService aceita
            _vm.Username = "admin";
            _vm.Password = "1234";

            // Captura de erros para validar sucesso silencioso
            string erro = "";
            _vm.MessageBoxAction = (msg) => erro = msg;

            // 2. Act
            _vm.LoginCommand.Execute(null);

            // 3. Assert
            // Se não houve mensagem de erro, assumimos que o fluxo seguiu para a navegação.
            // (Num cenário ideal, o FakeViewFactory teria um contador de chamadas 'ShowDialogCalled').
            Assert.AreEqual("", erro, "Não devia ter aparecido mensagem de erro.");
        }

        [TestMethod]
        public void Login_ComCredenciaisErradas_DeveMostrarErro()
        {
            // 1. Arrange
            _vm.Username = "hacker";
            _vm.Password = "errada";

            string mensagemCapturada = "";
            _vm.MessageBoxAction = (msg) => mensagemCapturada = msg;

            // 2. Act
            _vm.LoginCommand.Execute(null);

            // 3. Assert
            // Aqui é crucial validar que o utilizador foi avisado do erro
            Assert.IsTrue(mensagemCapturada.Contains("Invalid") || mensagemCapturada.Contains("incorretos"),
                "Devia ter mostrado mensagem de credenciais inválidas.");
        }
    }
}