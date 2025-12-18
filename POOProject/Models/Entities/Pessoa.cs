using System;

namespace POOProject.Models.Entities
{
    public abstract class Pessoa
    {
        private string _firstName;
        private string _lastName;

        // Propriedade com Validação
        public string FirstName
        {
            get => _firstName;
            private set // Só pode ser alterado internamente ou no construtor
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

        // Propriedade calculada (Computed Property)
        public string NomeCompleto => $"{FirstName} {LastName}";

        protected Pessoa(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        public void AtualizarDados(string novoNome, string novoApelido)
        {
            FirstName = novoNome;
            LastName = novoApelido;
        }
    }
}