using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using System;

namespace POOProjectTests.Models
{
    [TestClass]
    public class ClienteTests
    {
        [TestMethod]
        public void CriarCliente_ComDadosValidos_DeveSucesso()
        {
            var c = new Cliente("Maria", "Jose");
            Assert.AreEqual("Maria Jose", c.NomeCompleto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CriarCliente_SemNome_DeveLancarErro()
        {
            // Tentar criar sem primeiro nome -> Deve dar estoiro controlado
            new Cliente("", "Silva");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CriarCliente_SemSobrenome_DeveLancarErro()
        {
            // Tentar criar sem sobrenome -> Deve dar estoiro controlado
            new Cliente("Joao", "");
        }
    }
}