using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    public class Funcionario : Pessoa
    {
        public int Id { get; set; }
        public string Username { get; set; }

        // --- ADICIONADO: Agora o Funcionário guarda a sua própria password ---
        public string Password { get; set; }

        public Funcionario(string firstName, string lastName, int id)
            : base(firstName, lastName) // Chama o construtor da Pessoa
        {
            Id = id;
            // Gera um username default (pode ser alterado depois)
            Username = $"{firstName}.{lastName}".ToLower();
            Password = "123"; // Password temporária por defeito (opcional)
        }
    }
}