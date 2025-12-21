using POOProject.Models.Entities;
using System.Collections.Generic; // Necessário para List<>

namespace POOProject.Models.Repositories.Interfaces
{
    /// <summary>
    /// Contrato para acesso aos dados dos Funcionários.
    /// O uso desta interface permite trocar o sistema de ficheiros (JSON) por SQL no futuro
    /// sem ter de alterar o resto da aplicação.
    /// </summary>
    public interface IFuncionarioRepository
    {
        // Pesquisa rápida para o Login e validação de registos duplicados
        Funcionario GetByUsername(string username);

        // Método inteligente: Se o ID existir atualiza, senão cria novo.
        void SaveOrUpdate(Funcionario funcionario);

        // Devolve a lista completa para preencher as DataGrids
        List<Funcionario> GetAll();
    }
}