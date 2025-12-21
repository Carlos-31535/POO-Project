using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using System;

namespace POOProjectTests.Models
{
    /// <summary>
    /// Testes para a entidade Cliente.
    /// Focados principalmente na validação de entrada de dados (Encapsulamento).
    /// </summary>
    [TestClass]
    public class ClienteTests
    {
        [TestMethod]
        public void CriarCliente_ComDadosValidos_DeveSucesso()
        {
            // Happy Path (Caminho Feliz)
            var c = new Cliente("Maria", "Jose");

            // Verifica se a propriedade computada NomeCompleto funciona bem
            Assert.AreEqual("Maria Jose", c.NomeCompleto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CriarCliente_SemNome_DeveLancarErro()
        {
            // Robustez: O sistema não deve permitir clientes sem nome.
            // O teste passa se a exceção for lançada (ExpectedException).
            new Cliente("", "Silva");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CriarCliente_SemSobrenome_DeveLancarErro()
        {
            // Robustez: O mesmo para o apelido.
            new Cliente("Joao", "");
        }
    }
}