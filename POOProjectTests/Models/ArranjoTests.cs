using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using System;
// Já não precisas obrigatoriamente do System.Linq para aceder à lista, mas dá jeito.

namespace POOProjectTests.Models
{
    [TestClass]
    public class ArranjoTests
    {
        [TestMethod]
        public void AdicionarCalcado_ComDadosValidos_DeveAdicionarComSucesso()
        {
            // 1. Arrange
            var cliente = new Cliente("Adriana", "Gomes");
            var funcionario = new Funcionario("Joao", "Silva", 1);
            Arranjo a = new Arranjo(cliente, funcionario);

            Calcado c = new Calcado { NumPar = "Par 1" };
            c.ServicosParaFazer.Add(Servicos.Colar); // OBRIGATÓRIO

            // 2. Act
            a.AdicionarCalcado(c);

            // 3. Assert
            Assert.AreEqual(1, a.QuantidadePares);
            // AGORA PODES FAZER ISTO (Mais simples):
            Assert.AreEqual("Par 1", a.ListaCalcado[0].NumPar);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CriarArranjo_SemCliente_DeveDarErro()
        {
            var funcionario = new Funcionario("Joao", "Silva", 1);
            new Arranjo(null, funcionario);
        }

        [TestMethod]
        public void NovoArranjo_DeveComecarComEstadoArranjar()
        {
            Arranjo a = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            Assert.AreEqual(EstadoArranjo.Arranjar, a.Estado);
        }

        [TestMethod]
        public void QuantidadePares_ComVariosItens_DeveSomarCorretamente()
        {
            // 1. Arrange
            var a = new Arranjo(new Cliente("T", "U"), new Funcionario("F", "T", 1));

            var s1 = new Calcado { NumPar = "Par 1" }; s1.ServicosParaFazer.Add(Servicos.Forma);
            var s2 = new Calcado { NumPar = "Par 2" }; s2.ServicosParaFazer.Add(Servicos.Capas);
            var s3 = new Calcado { NumPar = "Par 3" }; s3.ServicosParaFazer.Add(Servicos.Colar);

            // 2. Act
            a.AdicionarCalcado(s1);
            a.AdicionarCalcado(s2);
            a.AdicionarCalcado(s3);

            // 3. Assert
            Assert.AreEqual(3, a.QuantidadePares);
            // Testar index direto:
            Assert.AreEqual("Par 3", a.ListaCalcado[2].NumPar);
        }

        [TestMethod]
        public void CriarArranjo_DeveInicializarListaVazia()
        {
            Arranjo a = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            Assert.IsNotNull(a.ListaCalcado);
            Assert.AreEqual(0, a.ListaCalcado.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AdicionarCalcado_SemServicos_DeveDarErro()
        {
            Arranjo a = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            Calcado c = new Calcado { NumPar = "Par Vazio" }; // Sem serviços
            a.AdicionarCalcado(c);
        }




        [TestMethod]
        public void MarcarComoPronto_DeveAtualizarEstadoEDataConclusao()
        {
            // 1. Arrange
            var arranjo = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            arranjo.AdicionarCalcado(new Calcado { NumPar = "P1", ServicosParaFazer = { Servicos.Reforços } });

            // Garante que começou como Arranjar
            Assert.AreEqual(EstadoArranjo.Arranjar, arranjo.Estado);
            Assert.IsNull(arranjo.DataConclusao); // Ainda não tem data de fim

            // 2. Act
            arranjo.MarcarComoPronto();

            // 3. Assert
            Assert.AreEqual(EstadoArranjo.Pronto, arranjo.Estado, "O estado devia ter mudado para Pronto.");
            Assert.IsNotNull(arranjo.DataConclusao, "A data de conclusão devia ter sido preenchida automaticamente.");

            // Verifica se a data é recente (ex: foi preenchida no último minuto)
            var diferencaTempo = DateTime.Now - arranjo.DataConclusao.Value;
            Assert.IsTrue(diferencaTempo.TotalMinutes < 1, "A data de conclusão não parece ser a atual.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarcarComoPronto_SemSapatos_DeveDarErro()
        {
            // Tentar concluir um talão vazio não deve ser permitido
            var arranjo = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));

            // Act
            arranjo.MarcarComoPronto();
        }

        [TestMethod]
        public void EntregarAoCliente_SeEstiverPronto_DeveMudarParaEntregue()
        {
            // 1. Arrange
            var arranjo = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            arranjo.AdicionarCalcado(new Calcado { NumPar = "P1", ServicosParaFazer = { Servicos.Solas } });

            // Tem de estar pronto antes de entregar
            arranjo.MarcarComoPronto();

            // 2. Act
            arranjo.EntregarAoCliente();

            // 3. Assert
            Assert.AreEqual(EstadoArranjo.Entregue, arranjo.Estado);
        }

        // --- TESTE DE VALIDAÇÃO DE CLIENTE ---
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CriarCliente_SemNome_DeveDarErro()
        {
            // Isto valida a regra que fez o teu outro teste falhar antes!
            new Cliente("", "Sobrenome");
        }
    }
}
