using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace POOProjectTests.Fakes
{
    public class FakeFuncionarioRepo : IFuncionarioRepository
    {
        // AQUI ESTÁ A CORREÇÃO: Uma lista para guardar os dados em memória
        public List<Funcionario> _db = new List<Funcionario>();

        public List<Funcionario> GetAll()
        {
            return new List<Funcionario>(_db);
        }

        public Funcionario GetByUsername(string username)
        {
            // Agora procura a sério na lista
            return _db.FirstOrDefault(f => f.Username == username);
        }

        public void SaveOrUpdate(Funcionario funcionario)
        {
            // Se já existe, atualiza. Se não, adiciona.
            var existente = _db.FirstOrDefault(f => f.Username == funcionario.Username);
            if (existente != null)
            {
                _db.Remove(existente);
            }
            _db.Add(funcionario);
        }
    }
}