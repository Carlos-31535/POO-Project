using POOProject.Models.Enums;
using System.Collections.Generic;

namespace POOProject.Models.Entities
{
    public class Calcado
    {
        public string NumPar { get; set; }
        public TipoCalcado Tipo { get; set; }
        public CorCalcado Cor { get; set; }
        public string Descricao { get; set; }

        public List<Servicos> ServicosParaFazer { get; set; }

        public Calcado()
        {
            ServicosParaFazer = new List<Servicos>();
        }

        // Método auxiliar para apresentação
        public string ResumoServicos => string.Join(", ", ServicosParaFazer);
    }
}