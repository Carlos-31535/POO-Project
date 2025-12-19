using POOProject.Models.Entities;

namespace POOProject.ViewModels.Interfaces
{
    public interface IAuthenticationService
    {
        // Verifica login na tabela de Funcionários
        bool Authenticate(string username, string password);

        // Regista um novo Funcionário diretamente 
        bool RegisterFuncionario(string username, string password, string firstName, string lastName);

        Funcionario? CurrentFuncionario { get; }
    }
}