using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace POOProjectTests.Fakes
{
    /// <summary>
    /// Repositório "Falso" para testes unitários.
    /// Em vez de gravar num ficheiro JSON (lento e difícil de limpar),
    /// guarda os dados numa Lista em memória RAM. 
    /// Isto permite que cada teste comece com uma base de dados limpa.
    /// </summary>
    public class FakeFuncionarioRepo : IFuncionarioRepository
    {
        // A nossa "Base de Dados" temporária
        public List<Funcionario> _db = new List<Funcionario>();

        public List<Funcionario> GetAll()
        {
            // Retorna uma cópia da lista para simular o comportamento real
            return new List<Funcionario>(_db);
        }

        public Funcionario GetByUsername(string username)
        {
            // Simula a pesquisa na base de dados
            return _db.FirstOrDefault(f => f.Username == username);
        }

        public void SaveOrUpdate(Funcionario funcionario)
        {
            // Lógica simplificada de "Upsert" (Update ou Insert)
            var existente = _db.FirstOrDefault(f => f.Username == funcionario.Username);

            if (existente != null)
            {
                _db.Remove(existente); // Remove o antigo
            }

            _db.Add(funcionario); // Adiciona o novo/atualizado
        }
    }
}