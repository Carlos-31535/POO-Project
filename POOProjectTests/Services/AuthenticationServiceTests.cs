using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.ViewModels.Services;
using POOProjectTests.Fakes;

namespace POOProjectTests.Services
{
    /// <summary>
    /// Testes de Integração da Lógica de Negócio.
    /// Aqui testamos o "AuthenticationService" verdadeiro, mas ligamo-lo a um Repositório Falso.
    /// Isto garante que testamos a lógica do código sem depender de ficheiros ou bases de dados reais.
    /// </summary>
    [TestClass]
    public class AuthenticationServiceTests
    {
        private FakeFuncionarioRepo _repo;
        private AuthenticationService _service;

        // O [TestInitialize] corre antes de CADA teste individual.
        // Isto garante que cada teste começa com uma "base de dados" limpa e vazia.
        [TestInitialize]
        public void Setup()
        {
            _repo = new FakeFuncionarioRepo();

            // Injeção de Dependência manual para testes
            _service = new AuthenticationService(_repo);

            // Nota: Se o Logger estiver implementado, injetaríamos aqui o FakeLogger:
            // _service = new AuthenticationService(_repo, new FakeLogger());
        }

        [TestMethod]
        public void Authenticate_UserExistente_DeveRetornarTrue()
        {
            // 1. ARRANGE (Preparação)
            // Criamos um cenário onde o utilizador já existe no sistema
            var user = new Funcionario("Joao", "Teste", 1) { Username = "joao", Password = "123" };
            _repo.SaveOrUpdate(user);

            // 2. ACT (Ação)
            // Tentamos fazer login com as credenciais corretas
            bool resultado = _service.Authenticate("joao", "123");

            // 3. ASSERT (Verificação)
            Assert.IsTrue(resultado, "O login devia ter sido aceite.");

            // Verifica se o serviço guardou o utilizador em sessão (estado global)
            Assert.IsNotNull(_service.CurrentFuncionario);
            Assert.AreEqual("Joao", _service.CurrentFuncionario.FirstName);
        }

        [TestMethod]
        public void Authenticate_PasswordErrada_DeveRetornarFalse()
        {
            // 1. Arrange
            var user = new Funcionario("Joao", "Teste", 1) { Username = "joao", Password = "123" };
            _repo.SaveOrUpdate(user);

            // 2. Act
            // Tentamos login com password errada
            bool resultado = _service.Authenticate("joao", "errada");

            // 3. Assert
            Assert.IsFalse(resultado, "O login devia ter falhado.");
            Assert.IsNull(_service.CurrentFuncionario, "Não devia haver utilizador logado.");
        }

        [TestMethod]
        public void RegisterFuncionario_NovoUser_DeveGravarNoRepo()
        {
            // Neste teste verificamos se o serviço chama corretamente o repositório para gravar.

            // Act
            bool resultado = _service.RegisterFuncionario("maria", "pass", "Maria", "Silva");

            // Assert
            Assert.IsTrue(resultado);

            // Validação Crítica: Vamos "espreitar" dentro do FakeRepo para ver se o user lá está.
            // Isto prova que a ligação entre Serviço -> Repositório está a funcionar.
            var userGravado = _repo.GetByUsername("maria");

            Assert.IsNotNull(userGravado, "O funcionário devia ter ficado gravado no repositório.");
            Assert.AreEqual("Maria", userGravado.FirstName);
        }
    }
}