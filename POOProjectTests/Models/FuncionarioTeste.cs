using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;

namespace POOProjectTests.Models
{
    /// <summary>
    /// Suite de testes para a entidade Funcionário.
    /// Valida a criação, herança e regras de negócio automáticas (como o username).
    /// </summary>
    [TestClass]
    public class FuncionarioTests
    {
        [TestMethod]
        public void CriarFuncionario_DeveGerarUsernameAutomatico()
        {
            // 1. Act (Ação)
            var func = new Funcionario("Pedro", "Santos", 1);

            // 2. Assert (Verificação)
            // Regra de Negócio: O sistema deve facilitar a vida ao utilizador e gerar
            // um login padrão (nome.apelido) automaticamente.
            Assert.AreEqual("pedro.santos", func.Username, "O username deve ser gerado automaticamente como 'nome.apelido'.");
        }

        [TestMethod]
        public void CriarFuncionario_DeveGuardarDadosCorretamente()
        {
            // 1. Arrange (Preparação)
            string nome = "Joao";
            string apelido = "Silva";
            int id = 10;

            // 2. Act
            Funcionario f = new Funcionario(nome, apelido, id);
            f.Username = "joao.silva"; // Simulamos uma alteração manual do username

            // 3. Assert
            // Verifica se o construtor preencheu tudo corretamente
            Assert.AreEqual("Joao", f.FirstName);
            Assert.AreEqual("Silva", f.LastName);
            Assert.AreEqual(10, f.Id);
            Assert.AreEqual("joao.silva", f.Username);
        }

        [TestMethod]
        public void NomeCompleto_DeveJuntarNomeEApelido()
        {
            // Este teste é importante porque valida a HERANÇA.
            // A propriedade 'NomeCompleto' vem da classe pai 'Pessoa', 
            // mas testamo-la aqui para garantir que o Funcionário a herdou bem.

            // 1. Arrange
            Funcionario f = new Funcionario("Maria", "Santos", 1);

            // 2. Act
            string nomeCompleto = f.NomeCompleto;

            // 3. Assert
            Assert.AreEqual("Maria Santos", nomeCompleto);
        }
    }
}