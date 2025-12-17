using POOProject.Models.Entities;

namespace POOProject.Models.Repositories.Interfaces
{
    public interface IFuncionarioRepository
    {
        // Encontra os dados do funcionário (Nome, Apelido) baseado no Username de login
        Funcionario GetByUsername(string username);

        // Guarda ou Atualiza um funcionário
        void SaveOrUpdate(Funcionario funcionario);

        List<Funcionario> GetAll();
    }
}