using POOProject.Models.Emuns;
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
            Estado = EstadoArranjo.Recebido; // Começa sempre como "Recebido"
            DataEntrada = DateTime.Now;
        }
    }
}
