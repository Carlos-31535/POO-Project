using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    public class Cliente : Pessoa
    {
        public Cliente(string firstName, string lastName)
            : base(firstName, lastName)
        {
        }
    }
}
