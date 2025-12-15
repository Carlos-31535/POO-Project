using System.IO;
using System.Text.Json;
using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;

namespace POOProject.Models.Repositories
{
    public class ArranjoRepository : IArranjoRepository
    {
        private readonly string _filePath;

        public ArranjoRepository()
        {
            // Cria a pasta Data na diretoria da aplicação se não existir
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            _filePath = Path.Combine(folder, "arranjos.json");

            // Se o ficheiro não existir, cria um array vazio []
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public void SaveArranjo(Arranjo arranjo)
        {
            var lista = GetAllArranjos();

            // 1. Gerar o ID automático (Ex: A0001)
            arranjo.Id = GenerateNextId(lista);

            // 2. Adicionar à lista
            lista.Add(arranjo);

            // 3. Gravar no JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(lista, options);
            File.WriteAllText(_filePath, json);
        }

        public void Update(Arranjo arranjoAtualizado)
        {
            var lista = GetAllArranjos();

            // 1. Procura a posição do arranjo na lista pelo ID
            int index = lista.FindIndex(a => a.Id == arranjoAtualizado.Id);

            if (index != -1)
            {
                // 2. Substitui o antigo pelo novo (já com o estado "Pronto")
                lista[index] = arranjoAtualizado;

                // 3. Regrava o ficheiro inteiro com a alteração
                SaveChanges(lista);
            }
        }
        private void SaveChanges(List<Arranjo> lista)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(lista, options);
            File.WriteAllText(_filePath, json);
        }

        public List<Arranjo> GetAllArranjos()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<Arranjo>();

                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Arranjo>>(json) ?? new List<Arranjo>();
            }
            catch
            {
                return new List<Arranjo>();
            }
        }

        // Lógica para gerar IDs sequenciais: A0001 -> A9999 -> B0001
        private string GenerateNextId(List<Arranjo> lista)
        {
            if (lista == null || lista.Count == 0) return "A0001";

            var ultimo = lista.Last();
            string ultimoId = ultimo.Id; // Ex: "A0015"

            if (string.IsNullOrEmpty(ultimoId) || ultimoId.Length < 2) return "A0001";

            char letra = ultimoId[0];
            string parteNumerica = ultimoId.Substring(1);

            if (int.TryParse(parteNumerica, out int numero))
            {
                numero++;
                // Se passar de 9999, muda de letra (A -> B) e reinicia o número
                if (numero > 9999)
                {
                    numero = 1;
                    letra++;
                }
                return $"{letra}{numero:D4}";
            }

            return "ERR00"; // Fallback de segurança
        }
    }
}