using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;

namespace POOProjectTests
{
    [TestClass]
    public class FuncionarioTests
    {
        [TestMethod]
        public void CriarFuncionario_DeveGerarUsernameAutomatico()
        {
            // 1. Act
            var func = new Funcionario("Pedro", "Santos", 1);

            // 2. Assert
            // A regra é: nome.sobrenome (tudo minusculo)
            Assert.AreEqual("pedro.santos", func.Username, "O username deve ser gerado automaticamente como 'nome.apelido'.");
        }

        [TestMethod]
        public void CriarFuncionario_DeveGuardarDadosCorretamente()
        {
            // 1. Arrange
            string nome = "Joao";
            string apelido = "Silva";
            int id = 10;

            // 2. Act
            Funcionario f = new Funcionario(nome, apelido, id);
            f.Username = "joao.silva"; // Simulamos a atribuição do username

            // 3. Assert
            Assert.AreEqual("Joao", f.FirstName);
            Assert.AreEqual("Silva", f.LastName);
            Assert.AreEqual(10, f.Id);
            Assert.AreEqual("joao.silva", f.Username);
        }

        [TestMethod]
        public void NomeCompleto_DeveJuntarNomeEApelido()
        {
            // 1. Arrange
            Funcionario f = new Funcionario("Maria", "Santos", 1);

            // 2. Act
            string nomeCompleto = f.NomeCompleto; // Assume que tens esta propriedade na classe Pessoa/Funcionario

            // 3. Assert
            // Nota: Se a tua classe base Pessoa tiver "NomeCompleto", este teste valida isso.
            // Se for "FirstName + " " + LastName":
            Assert.AreEqual("Maria Santos", nomeCompleto);
        }
    }
}