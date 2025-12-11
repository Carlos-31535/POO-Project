using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Entities
{
    public class Pessoa
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Pessoa(string firstname, string lastname)
        {
            FirstName = firstname;
            LastName = lastname;

        }
    }
}
