using POOProject.Models.Entities;
using POOProject.ViewModels.Interfaces;

namespace POOProjectTests
{
    public class FakeAuthService : IAuthenticationService
    {
        public Funcionario CurrentFuncionario { get; set; }

        // --- ESTE ERA O QUE FALTAVA ---
        // Atualizámos para corresponder à nova interface
        public bool RegisterFuncionario(string username, string password, string firstName, string lastName)
        {
            // Simula que o registo funcionou sempre bem nos testes
            return true;
        }

        public bool Authenticate(string username, string password)
        {
            // Simula que apenas o user "admin" com pass "1234" é válido
            if (username == "admin" && password == "1234")
            {
                // Simulamos que ao fazer login, o sistema carrega o funcionário
                CurrentFuncionario = new Funcionario("Administrador", "Sistema", 1);
                return true;
            }

            return false;
        }
    }
}