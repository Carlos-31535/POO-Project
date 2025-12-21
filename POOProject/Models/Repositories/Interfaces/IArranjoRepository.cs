using POOProject.Models.Entities;
using System.Collections.Generic;

namespace POOProject.Models.Repositories.Interfaces
{
    /// <summary>
    /// Contrato para gestão dos Talões de Arranjo.
    /// Define as operações básicas de persistência (Gravar, Ler, Atualizar).
    /// </summary>
    public interface IArranjoRepository
    {
        void SaveArranjo(Arranjo arranjo);

        List<Arranjo> GetAllArranjos();

        // Usado para mudar estados (ex: marcar como Pronto ou Entregue)
        void Update(Arranjo arranjo);
    }
}