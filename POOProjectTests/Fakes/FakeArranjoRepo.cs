using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using System.Collections.Generic;

namespace POOProjectTests.Fakes
{
    // Simula a base de dados de Arranjos em memória (List)
    public class FakeArranjoRepo : IArranjoRepository
    {
        // Deixamos público para o teste poder ver o que foi gravado
        public List<Arranjo> ArranjosGravados = new List<Arranjo>();

        public void SaveArranjo(Arranjo arranjo)
        {
            ArranjosGravados.Add(arranjo);
        }

        public List<Arranjo> GetAllArranjos()
        {
            // Retorna uma cópia da lista para simular o comportamento real
            return new List<Arranjo>(ArranjosGravados);
        }

        public void Update(Arranjo arranjo)
        {
            var index = ArranjosGravados.FindIndex(a => a.Id == arranjo.Id);
            if (index != -1)
            {
                ArranjosGravados[index] = arranjo;
            }
        }
    }
}