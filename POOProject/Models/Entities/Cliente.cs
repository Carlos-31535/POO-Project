using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    /// <summary>
    /// Representa o cliente final que requisita o serviço.
    /// Herda de 'Pessoa', aproveitando as validações de Nome e Apelido já existentes.
    /// </summary>
    public class Cliente : Pessoa
    {
        // NOTA DE DESENVOLVIMENTO:
        // Atualmente o Cliente não tem campos extra, mas mantemos a classe separada
        // porque no futuro pode se vir a querer utilizar mais campos

        public Cliente(string firstName, string lastName)
            : base(firstName, lastName) // Repassa os dados para o construtor da classe Pai
        {
            // Construtor limpo. Toda a lógica de validação de texto está na classe base 'Pessoa'.
        }
    }
}