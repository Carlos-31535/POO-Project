using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    /// <summary>
    /// Representa um utilizador do sistema (Staff).
    /// Herda de Pessoa para reaproveitar Nome/Apelido e validações.
    /// </summary>
    public class Funcionario : Pessoa
    {
        

        // Identificador numérico único (usado para ligar aos Arranjos)
        public int Id { get; set; }

       

        public string Username { get; set; }

        // A password fica guardada diretamente no funcionário para simplificar o login.
        // (Num sistema real, isto devia estar encriptado/hash).
        public string Password { get; set; }

     

        public Funcionario(string firstName, string lastName, int id)
            : base(firstName, lastName) // Passa os nomes para a classe Pai validar
        {
            Id = id;

            // Lógica de conveniência:
            // Gera automaticamente um username (ex: "joao.silva") para não ter de escrever sempre.
            Username = $"{firstName}.{lastName}".ToLower();

            // Password inicial por defeito para agilizar a criação de contas.
            // O funcionário pode mudar depois (se implementarmos essa funcionalidade).
            Password = "123";
        }
    }
}