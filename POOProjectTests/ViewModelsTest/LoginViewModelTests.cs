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
            _authService = new FakeAuthService();
            _viewFactory = new FakeViewFactory();
            _vm = new LoginViewModel(_authService, _viewFactory);

            // Silenciar mensagens
            _vm.MessageBoxAction = (msg) => { };
        }

        [TestMethod]
        public void Login_ComCredenciaisValidas_DeveSucesso()
        {
            // 1. Arrange - Dados corretos (definidos no FakeAuthService)
            _vm.Username = "admin";
            _vm.Password = "1234";

            bool janelaAbriu = false;
            // "Hack" simples: se o método ShowDialog do FakeViewFactory for chamado, sabemos que o login passou.
            // Para isto funcionar, teríamos de adicionar lógica ao FakeViewFactory, 
            // mas podemos testar indiretamente: se NÃO deu erro, assumimos sucesso no fluxo simples.

            // Uma forma melhor é capturar a mensagem de erro. Se não houver mensagem, é sucesso.
            string erro = "";
            _vm.MessageBoxAction = (msg) => erro = msg;

            // 2. Act
            _vm.LoginCommand.Execute(null);

            // 3. Assert
            Assert.AreEqual("", erro, "Não devia ter aparecido mensagem de erro.");
        }

        [TestMethod]
        public void Login_ComCredenciaisErradas_DeveMostrarErro()
        {
            // 1. Arrange - Dados errados
            _vm.Username = "hacker";
            _vm.Password = "errada";

            string mensagemCapturada = "";
            _vm.MessageBoxAction = (msg) => mensagemCapturada = msg;

            // 2. Act
            _vm.LoginCommand.Execute(null);

            // 3. Assert
            Assert.IsTrue(mensagemCapturada.Contains("Invalid"), "Devia ter mostrado mensagem de credenciais inválidas.");
        }
    }
}