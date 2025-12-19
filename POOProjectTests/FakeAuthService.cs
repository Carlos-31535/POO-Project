using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Interfaces;

namespace POOProjectTests
{
    public class FakeAuthService : IAuthenticationService
    {
        // --- AQUI ESTAVA O ERRO: Faltava esta propriedade ---
        public Funcionario CurrentFuncionario { get; set; }

        public bool CreateUser(string username, string password, string confirmPassword)
        {
            return true; // Simula sucesso no registo
        }

        public bool UserExists(string username, string password)
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