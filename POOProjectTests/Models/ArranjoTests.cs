using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using System;

namespace POOProjectTests.Models
{
    /// <summary>
    /// Testes fundamentais do negócio (Core Business Logic).
    /// Valida o ciclo de vida de um arranjo, desde a entrega até ao levantamento,
    /// garantindo que não há estados inválidos.
    /// </summary>
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

            // Cria um sapato válido com pelo menos um serviço
            Calcado c = new Calcado { NumPar = "Par 1" };
            c.ServicosParaFazer.Add(Servicos.Colar);

            // 2. Act
            a.AdicionarCalcado(c);

            // 3. Assert
            Assert.AreEqual(1, a.QuantidadePares);
            // Verifica se o objeto guardado na lista é exatamente o que criámos
            Assert.AreEqual("Par 1", a.ListaCalcado[0].NumPar);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CriarArranjo_SemCliente_DeveDarErro()
        {
            // Regra de Integridade: Não podem existir talões "órfãos" (sem cliente).
            var funcionario = new Funcionario("Joao", "Silva", 1);

            // Deve lançar exceção imediatamente
            new Arranjo(null, funcionario);
        }

        [TestMethod]
        public void NovoArranjo_DeveComecarComEstadoArranjar()
        {
            // O estado inicial default é crucial para o workflow correto.
            Arranjo a = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));

            Assert.AreEqual(EstadoArranjo.Arranjar, a.Estado);
        }

        [TestMethod]
        public void QuantidadePares_ComVariosItens_DeveSomarCorretamente()
        {
            // Teste de carga simples (vários itens)

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
            // Verifica se a ordem de inserção foi mantida
            Assert.AreEqual("Par 3", a.ListaCalcado[2].NumPar);
        }

        [TestMethod]
        public void CriarArranjo_DeveInicializarListaVazia()
        {
            // Evita o erro comum de "Object reference not set to an instance of an object"
            // ao tentar adicionar sapatos numa lista que é null.
            Arranjo a = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));

            Assert.IsNotNull(a.ListaCalcado);
            Assert.AreEqual(0, a.ListaCalcado.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AdicionarCalcado_SemServicos_DeveDarErro()
        {
            // Regra de Negócio: Não faz sentido aceitar um sapato se não for para fazer nada.
            Arranjo a = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            Calcado c = new Calcado { NumPar = "Par Vazio" }; // Lista de serviços vazia

            a.AdicionarCalcado(c);
        }

        [TestMethod]
        public void MarcarComoPronto_DeveAtualizarEstadoEDataConclusao()
        {
            // 1. Arrange
            var arranjo = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            arranjo.AdicionarCalcado(new Calcado { NumPar = "P1", ServicosParaFazer = { Servicos.Reforços } });

            Assert.AreEqual(EstadoArranjo.Arranjar, arranjo.Estado);
            Assert.IsNull(arranjo.DataConclusao); // Pre-condição: Data vazia

            // 2. Act
            arranjo.MarcarComoPronto();

            // 3. Assert
            Assert.AreEqual(EstadoArranjo.Pronto, arranjo.Estado, "O estado devia ter mudado para Pronto.");
            Assert.IsNotNull(arranjo.DataConclusao, "A data de conclusão devia ser preenchida.");

            // Valida se a data registada é realmente a de "agora" (margem de 1 minuto)
            var diferencaTempo = DateTime.Now - arranjo.DataConclusao.Value;
            Assert.IsTrue(diferencaTempo.TotalMinutes < 1, "A data de conclusão deve ser a data atual.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MarcarComoPronto_SemSapatos_DeveDarErro()
        {
            // Impedir fechar talões que não têm trabalho associado (erro humano).
            var arranjo = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));

            // Act -> Deve falhar
            arranjo.MarcarComoPronto();
        }

        [TestMethod]
        public void EntregarAoCliente_SeEstiverPronto_DeveMudarParaEntregue()
        {
            // Testa o Workflow completo: Arranjar -> Pronto -> Entregue

            // 1. Arrange
            var arranjo = new Arranjo(new Cliente("A", "B"), new Funcionario("F", "T", 1));
            arranjo.AdicionarCalcado(new Calcado { NumPar = "P1", ServicosParaFazer = { Servicos.Solas } });

            // Tem de estar pronto antes de entregar (pré-requisito)
            arranjo.MarcarComoPronto();

            // 2. Act
            arranjo.EntregarAoCliente();

            // 3. Assert
            Assert.AreEqual(EstadoArranjo.Entregue, arranjo.Estado);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CriarCliente_SemNome_DeveDarErro()
        {
            // Teste de Integração indireto:
            // Validamos que mesmo ao criar um Arranjo, as regras do Cliente continuam ativas.
            new Cliente("", "Sobrenome");
        }
    }
}