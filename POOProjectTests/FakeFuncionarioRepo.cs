using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using System.Collections.Generic;

namespace POOProjectTests
{
    public class FakeFuncionarioRepo : IFuncionarioRepository
    {
        public List<Funcionario> GetAll()
        {
            // Retorna um funcionário fictício apenas para as listas não estarem vazias
            return new List<Funcionario>
            {
                new Funcionario("Func", "Teste", 99)
            };
        }

        public Funcionario GetByUsername(string username) => null;

        public void SaveOrUpdate(Funcionario funcionario) { }
    }
}