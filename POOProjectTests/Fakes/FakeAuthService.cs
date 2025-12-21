using POOProject.Models.Entities;
using POOProject.ViewModels.Interfaces;

namespace POOProjectTests.Fakes
{
    /// <summary>
    /// Serviço de Autenticação Falso.
    /// Permite testar o LoginViewModel sem depender da base de dados de utilizadores reais.
    /// </summary>
    public class FakeAuthService : IAuthenticationService
    {
        public Funcionario CurrentFuncionario { get; set; }

        public bool RegisterFuncionario(string username, string password, string firstName, string lastName)
        {
            // Para efeitos de teste do ViewModel, assumimos que o registo funciona sempre.
            // Se quiséssemos testar falhas, poderíamos mudar isto para retornar false se username == "erro".
            return true;
        }

        public bool Authenticate(string username, string password)
        {
            // Hardcoded para testes controlados.
            // Sabemos sempre que "admin" + "1234" tem de dar sucesso.
            if (username == "admin" && password == "1234")
            {
                CurrentFuncionario = new Funcionario("Administrador", "Sistema", 1);
                return true;
            }

            return false;
        }
    }
}