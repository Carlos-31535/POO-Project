using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using System.Collections.Generic;

namespace POOProjectTests.Fakes
{
    /// <summary>
    /// Simula a persistência de Arranjos.
    /// Útil para verificar se o botão "Gravar" do ViewModel chamou realmente o repositório.
    /// </summary>
    public class FakeArranjoRepo : IArranjoRepository
    {
        // Deixamos público propositadamente para o Teste Unitário poder consultar (Spy).
        // Exemplo de teste: Assert.AreEqual(1, repo.ArranjosGravados.Count);
        public List<Arranjo> ArranjosGravados = new List<Arranjo>();

        public void SaveArranjo(Arranjo arranjo)
        {
            ArranjosGravados.Add(arranjo);
        }

        public List<Arranjo> GetAllArranjos()
        {
            return new List<Arranjo>(ArranjosGravados);
        }

        public void Update(Arranjo arranjo)
        {
            // Simula a atualização de estado (ex: marcar como Pronto)
            var index = ArranjosGravados.FindIndex(a => a.Id == arranjo.Id);
            if (index != -1)
            {
                ArranjosGravados[index] = arranjo;
            }
        }
    }
}