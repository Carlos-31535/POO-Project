using POOProject.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOProject.Models.Entities
{
    public class Arranjo
    {
        // --- PROPRIEDADES (Tudo Public { get; set; } para o JSON funcionar bem) ---

        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        public Cliente Cliente { get; set; }
        public Funcionario FuncionarioResponsavel { get; set; }

        public DateTime DataEntrada { get; set; }
        public DateTime? DataConclusao { get; set; }
        public EstadoArranjo Estado { get; set; }

        // [CORREÇÃO] Transformámos a lista numa Propriedade com { get; set; }
        // Assim o JSON grava e lê os sapatos automaticamente.
        public List<Calcado> ListaCalcado { get; set; } = new List<Calcado>();

        // --- CONSTRUTORES ---

        // Construtor Principal
        public Arranjo(Cliente cliente, Funcionario funcionario)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            FuncionarioResponsavel = funcionario ?? throw new ArgumentNullException(nameof(funcionario));

            ListaCalcado = new List<Calcado>();
            DataEntrada = DateTime.Now;
            Estado = EstadoArranjo.Arranjar;
        }

        // Construtor Vazio (JSON)
        public Arranjo()
        {
            ListaCalcado = new List<Calcado>();
            Cliente = null!;
            FuncionarioResponsavel = null!;
        }

        // --- MÉTODOS DE NEGÓCIO ---

        public void AdicionarCalcado(Calcado calcado)
        {
            // Validação de segurança
            if (calcado == null) throw new ArgumentNullException("Sapato inválido.");
            if (calcado.ServicosParaFazer.Count == 0)
                throw new InvalidOperationException("O sapato tem de ter pelo menos 1 serviço.");

            ListaCalcado.Add(calcado);
        }

        public void MarcarComoPronto()
        {
            if (ListaCalcado.Count == 0)
                throw new InvalidOperationException("Não podes fechar um talão sem sapatos.");

            Estado = EstadoArranjo.Pronto;
            DataConclusao = DateTime.Now;
        }

        public void EntregarAoCliente()
        {
            if (Estado != EstadoArranjo.Pronto)
                throw new InvalidOperationException("O arranjo ainda não está pronto.");

            Estado = EstadoArranjo.Entregue;
        }

        // --- HELPERS VISUAIS (Para a DataGrid do Main Window) ---

        // Quantidade de pares (seguro contra nulls)
        public int QuantidadePares => ListaCalcado?.Count ?? 0;

        // Nome do cliente formatado
        public string NomeClienteDisplay => Cliente?.NomeCompleto ?? "N/A";
    }
}