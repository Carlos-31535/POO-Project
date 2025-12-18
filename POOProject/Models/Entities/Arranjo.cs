using POOProject.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    public class Arranjo
    {
        public string Id { get; set; } // O ID gerado automaticamente (Ex: "A0001")
        public Cliente Cliente { get; set; }
        public Funcionario FuncionarioResponsavel { get; set; } // Quem atendeu o cliente
        public DateTime DataEntrada { get; set; }
        public EstadoArranjo Estado { get; set; } // O estado do arranjo 

        public List<Calcado> ListaCalcado { get; set; } // A lista de calçado deste talão(caso tenha mais que 1 par)

        public Arranjo()
        {
            ListaCalcado = new List<Calcado>();
            Estado = EstadoArranjo.Arranjar; // Começa sempre como "Arranjar"
            DataEntrada = DateTime.Now;
        }

        /// <summary>
        /// Adiciona um par de sapatos, mas valida antes se o Cliente e Funcionário estão preenchidos.
        /// </summary>
        public void AdicionarCalcado(Calcado calcado)
        {
            // 1. Validar se o Sapato existe
            if (calcado == null)
            {
                throw new ArgumentNullException("Não podes adicionar um registo de calçado vazio.");
            }

            // 2. Validar se o Cliente existe e tem nomes
            if (this.Cliente == null)
            {
                throw new InvalidOperationException("Erro: Tens de selecionar um Cliente antes de adicionar sapatos.");
            }

            if (string.IsNullOrWhiteSpace(this.Cliente.FirstName) || string.IsNullOrWhiteSpace(this.Cliente.LastName))
            {
                throw new InvalidOperationException("O Cliente selecionado tem de ter Nome e Apelido válidos.");
            }

            // 3. Validar se o Funcionário Responsável existe
            if (this.FuncionarioResponsavel == null)
            {
                throw new InvalidOperationException("Erro: O talão tem de ter um Funcionário responsável associado.");
            }

            // --- Se passou por tudo isto, adiciona à lista ---
            this.ListaCalcado.Add(calcado);
        }

        /// <summary>
        /// Calcula quantos pares existem neste talão.
        /// Útil para mostrar na grelha ou no recibo.
        /// </summary>
        public int ObterQuantidadePares()
        {
            return ListaCalcado.Count;
        }
    }
}
