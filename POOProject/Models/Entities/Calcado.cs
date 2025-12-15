using POOProject.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    public class Calcado
    {
        public string NumPar { get; set; } // Ex: "Par 1"
        public TipoCalcado Tipo { get; set; } // Sapatilha, Bota...
        public CorCalcado Cor { get; set; }   // Preto, Castanho...
        public string Descricao { get; set; } // Notas extra (Ex: "Marca Nike")

        // A lista de serviços para ESTE par (Ex: Capas, Colar)
        public List<Servicos> ServicosParaFazer { get; set; }

        public Calcado()
        {
            ServicosParaFazer = new List<Servicos>();
        }
    }
}
