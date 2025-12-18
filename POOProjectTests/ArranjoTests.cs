using Microsoft.VisualStudio.TestTools.UnitTesting;
using POOProject.Models.Entities;
using POOProject.Models.Enums;
using System;

namespace POOProjectTests
{
    [TestClass]
    public class ArranjoTests
    {
        // --- TESTE 1: O TEU TESTE ORIGINAL (SUCESSO) ---
        [TestMethod]
        public void AdicionarCalcado_ComClienteEFuncionario_DeveAdicionarComSucesso()
        {
            // 1. Arrange
            Arranjo a = new Arranjo();
            a.Cliente = new Cliente("Adriana", "Gomes");
            a.FuncionarioResponsavel = new Funcionario("Joao", "Silva", 1);

            Calcado c = new Calcado { NumPar = "Par 1" };

            // 2. Act
            a.AdicionarCalcado(c);

            // 3. Assert
            Assert.AreEqual(1, a.ObterQuantidadePares());
            Assert.IsNotNull(a.ListaCalcado);
        }

        // --- TESTE 2: O TEU TESTE DE ERRO (SEGURANÇA) ---
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AdicionarCalcado_SemCliente_DeveDarErro()
        {
            // 1. Arrange
            Arranjo a = new Arranjo();
            // NÃO definimos o cliente de propósito
            Calcado c = new Calcado { NumPar = "Par 1" };

            // 2. Act (Vai rebentar aqui, e o teste passa se rebentar)
            a.AdicionarCalcado(c);
        }

        // --- TESTE 3: NOVO - VERIFICAR ESTADO INICIAL ---
        [TestMethod]
        public void NovoArranjo_DeveComecarComEstadoArranjar()
        {
            // 1. Arrange & Act
            Arranjo a = new Arranjo();

            // 2. Assert
            // Um talão novo nunca deve começar como "Pronto" ou "Entregue"
            Assert.AreEqual(EstadoArranjo.Arranjar, a.Estado, "O estado inicial deve ser 'Arranjar'.");
        }

        // --- TESTE 4: NOVO - CONTAR VÁRIOS ITENS ---
        [TestMethod]
        public void ObterQuantidadePares_ComVariosItens_DeveSomarCorretamente()
        {
            // 1. Arrange
            Arranjo a = new Arranjo();
            a.Cliente = new Cliente("Teste", "User");
            a.FuncionarioResponsavel = new Funcionario("Func", "Teste", 1);

            // 2. Act - Adicionar 3 pares
            a.AdicionarCalcado(new Calcado { NumPar = "Par 1" });
            a.AdicionarCalcado(new Calcado { NumPar = "Par 2" });
            a.AdicionarCalcado(new Calcado { NumPar = "Par 3" });

            // 3. Assert
            Assert.AreEqual(3, a.ObterQuantidadePares());
            Assert.AreEqual("Par 3", a.ListaCalcado[2].NumPar); // Verifica se o último ficou guardado
        }
        [TestMethod]
        public void CriarArranjo_DeveInicializarListaVazia()
        {
            // 1. Arrange & Act (Preparar e Agir)
            Arranjo a = new Arranjo();

            // 2. Assert (Verificar)
            Assert.IsNotNull(a.ListaCalcado, "A lista de calçado não devia ser nula");
            Assert.AreEqual(0, a.ListaCalcado.Count, "O arranjo novo devia vir sem sapatos");
            Assert.AreEqual(EstadoArranjo.Arranjar, a.Estado, "O estado inicial devia ser 'Arranjar'");
        }

        [TestMethod]
        public void AdicionarCalcado_DeveAumentarTamanhoDaLista()
        {
            // 1. Arrange
            Arranjo a = new Arranjo();
            Calcado sapato = new Calcado
            {
                NumPar = "Par 1",
                Tipo = TipoCalcado.Sapato,
                Cor = CorCalcado.Preto
            };

            // 2. Act
            a.ListaCalcado.Add(sapato);

            // 3. Assert
            Assert.AreEqual(1, a.ListaCalcado.Count, "A lista devia ter 1 elemento");
            Assert.AreEqual("Par 1", a.ListaCalcado[0].NumPar);
        }
    }
}