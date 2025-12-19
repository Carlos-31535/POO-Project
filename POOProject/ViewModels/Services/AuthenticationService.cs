using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Interfaces;
using System;

namespace POOProject.ViewModels.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // MUDANÇA: Só precisamos deste repositório! O UserRepository morreu. 💀
        private readonly IFuncionarioRepository _funcionarioRepository;

        public Funcionario? CurrentFuncionario { get; private set; }

        public AuthenticationService(IFuncionarioRepository funcionarioRepository)
        {
            _funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public bool Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            // Vai buscar o funcionário diretamente pelo username
            var func = _funcionarioRepository.GetByUsername(username);

            // Verifica se existe e a pass bate certo
            if (func != null && func.Password == password)
            {
                CurrentFuncionario = func;
                return true;
            }
            return false;
        }

        public bool RegisterFuncionario(string username, string password, string firstName, string lastName)
        {
            // 1. Verificar se o username já está ocupado
            if (_funcionarioRepository.GetByUsername(username) != null)
                return false;

            try
            {
                // 2. Criar o Objeto FUNCIONÁRIO diretamente
                // O construtor obriga a ter Nome e Apelido, resolvendo o teu erro da classe Pessoa!
                // O ID (0) será gerado automaticamente pela base de dados/repositório
                var novoFuncionario = new Funcionario(firstName, lastName, 0);

                // Preencher dados de Login
                novoFuncionario.Username = username;
                novoFuncionario.Password = password;

                // 3. Gravar na tabela de funcionários
                _funcionarioRepository.SaveOrUpdate(novoFuncionario);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}