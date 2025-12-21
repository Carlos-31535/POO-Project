using System;

namespace POOProject.Models.Entities
{
    /// <summary>
    /// Classe base para qualquer ser humano no sistema (Cliente ou Funcionário).
    /// É 'abstract' para garantir que nunca criamos uma "Pessoa" solta no programa 
    /// (tem de ser sempre ou um Cliente ou um Funcionário).
    /// </summary>
    public abstract class Pessoa
    {
        // --- CAMPOS PRIVADOS (Encapsulamento) ---
        // Guardam os dados reais, protegidos do acesso direto.
        private string _firstName;
        private string _lastName;

        // --- PROPRIEDADES COM VALIDAÇÃO ---

        public string FirstName
        {
            get => _firstName;
            // 'private set' obriga a passar pelo construtor ou métodos específicos.
            // Impede que alguém faça pessoa.FirstName = "" por engano no meio do código.
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O Nome não pode ser vazio.");
                _firstName = value;
            }
        }

        public string LastName
        {
            get => _lastName;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O Apelido não pode ser vazio.");
                _lastName = value;
            }
        }

        // --- HELPERS ---

        // Propriedade calculada (Computed Property).
        // Não guarda dados, apenas formata "Nome + Apelido" na hora para facilitar a exibição nas listas.
        public string NomeCompleto => $"{FirstName} {LastName}";

        // --- CONSTRUTOR ---

        // É 'protected' porque a classe é abstrata. Só os filhos (Cliente/Funcionario) podem chamar este construtor.
        protected Pessoa(string firstName, string lastName)
        {
            // Ao usar as propriedades (com letra grande), ativamos as validações logó na criação.
            FirstName = firstName;
            LastName = lastName;
        }

        // --- MÉTODOS ---

        // Permite alterar o nome mais tarde, mantendo a segurança das validações.
        public void AtualizarDados(string novoNome, string novoApelido)
        {
            FirstName = novoNome;
            LastName = novoApelido;
        }
    }
}