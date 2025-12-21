using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Interfaces;
using System;

namespace POOProject.ViewModels.Services
{
    /// <summary>
    /// Implementação real da lógica de autenticação.
    /// Atua como intermediário entre a Interface Gráfica e os Dados (Repositório).
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        // Dependência do repositório (não acedemos ao ficheiro diretamente, pedimos ao repo)
        private readonly IFuncionarioRepository _funcionarioRepository;

        // Propriedade para saber quem está logado em qualquer parte da app
        public Funcionario? CurrentFuncionario { get; private set; }

        // Injeção de Dependência no construtor
        public AuthenticationService(IFuncionarioRepository funcionarioRepository)
        {
            _funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public bool Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;

            // Busca o utilizador à base de dados
            var func = _funcionarioRepository.GetByUsername(username);

            // Verificação de segurança simples (match direto de strings)
            if (func != null && func.Password == password)
            {
                CurrentFuncionario = func;
                return true;
            }
            return false;
        }

        public bool RegisterFuncionario(string username, string password, string firstName, string lastName)
        {
            // Regra de Negócio 1: Não permitir usernames duplicados
            if (_funcionarioRepository.GetByUsername(username) != null)
                return false;

            try
            {
                // Criação da Entidade
                // O ID é passado a 0 porque o Repositório encarrega-se de gerar o ID correto.
                var novoFuncionario = new Funcionario(firstName, lastName, 0);

                novoFuncionario.Username = username;
                novoFuncionario.Password = password;

                // Persistência
                _funcionarioRepository.SaveOrUpdate(novoFuncionario);
                return true;
            }
            catch
            {
                // Em caso de erro (ex: falha no disco), devolve false para a UI avisar o utilizador
                return false;
            }
        }
    }
}