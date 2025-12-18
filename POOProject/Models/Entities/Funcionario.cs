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

        public Funcionario(string firstName, string lastName, int id)
            : base(firstName, lastName) // Chama o construtor da Pessoa (que valida os nomes)
        {
            Id = id;
            // Gera um username default se não for passado
            Username = $"{firstName}.{lastName}".ToLower();
        }
    }
}
