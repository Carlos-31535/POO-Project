using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.ViewModels.Services;
using POOProjectTests.Fakes;


namespace POOProjectTests.Services
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        private FakeFuncionarioRepo _repo;
        private AuthenticationService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new FakeFuncionarioRepo();
            _service = new AuthenticationService(_repo);
            // Nota: Se já implementaste os LOGS, tens de passar o logger fake aqui também:
            // _service = new AuthenticationService(_repo, new FakeLogger());
        }

        [TestMethod]
        public void Authenticate_UserExistente_DeveRetornarTrue()
        {
            // 1. Injetar um user no repositório falso
            var user = new Funcionario("Joao", "Teste", 1) { Username = "joao", Password = "123" };
            _repo.SaveOrUpdate(user);

            // 2. Testar o serviço real
            bool resultado = _service.Authenticate("joao", "123");

            // 3. Assert
            Assert.IsTrue(resultado);
            Assert.IsNotNull(_service.CurrentFuncionario);
            Assert.AreEqual("Joao", _service.CurrentFuncionario.FirstName);
        }

        [TestMethod]
        public void Authenticate_PasswordErrada_DeveRetornarFalse()
        {
            // 1. Injetar user
            var user = new Funcionario("Joao", "Teste", 1) { Username = "joao", Password = "123" };
            _repo.SaveOrUpdate(user);

            // 2. Tentar login com pass errada
            bool resultado = _service.Authenticate("joao", "errada");

            // 3. Assert
            Assert.IsFalse(resultado);
            Assert.IsNull(_service.CurrentFuncionario); // Não deve carregar o user
        }

        [TestMethod]
        public void RegisterFuncionario_NovoUser_DeveGravarNoRepo()
        {
            // Act
            bool resultado = _service.RegisterFuncionario("maria", "pass", "Maria", "Silva");

            // Assert
            Assert.IsTrue(resultado);

            // Verificar se ficou gravado no "banco de dados" fake
            var userGravado = _repo.GetByUsername("maria");
            Assert.IsNotNull(userGravado);
            Assert.AreEqual("Maria", userGravado.FirstName);
        }
    }
}