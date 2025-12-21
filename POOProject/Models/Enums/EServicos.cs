using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Enums
{
    /// <summary>
    /// Lista mestre de serviços que a loja disponibiliza e cobra.
    /// Usamos Enum para garantir que os nomes dos serviços saem sempre iguais nos talões.
    /// </summary>
    public enum Servicos
    {
        Forma,
        Capas,
        Solas,
        Tombas,
        Colar,
        Pintar,
        Cozer,
        Reforços
        // Nota: Novos serviços podem ser adicionados aqui no futuro sem afetar os dados antigos.
    }
}