using POOProject.Models.Entities;

namespace POOProject.ViewModels.Interfaces
{
    /// <summary>
    /// Contrato para o serviço de autenticação.
    /// O uso desta interface é crucial para a Injeção de Dependência, permitindo
    /// testar os ViewModels com um 'FakeAuthService' sem precisar da base de dados real.
    /// </summary>
    public interface IAuthenticationService
    {
        // Valida as credenciais de entrada
        bool Authenticate(string username, string password);

        // Cria e valida novos utilizadores no sistema
        bool RegisterFuncionario(string username, string password, string firstName, string lastName);

        // Mantém o estado da sessão atual (quem está logado)
        Funcionario? CurrentFuncionario { get; }
    }
}