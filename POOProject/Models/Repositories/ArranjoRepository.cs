using System; // Adicionado System para o AppDomain
using System.Collections.Generic;
using System.IO;
using System.Linq; // Adicionado para métodos LINQ (FindIndex, Last)
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
            // Configuração inicial do sistema de ficheiros
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            _filePath = Path.Combine(folder, "arranjos.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public void SaveArranjo(Arranjo arranjo)
        {
            var lista = GetAllArranjos();

            // 1. Lógica crítica: Gerar o ID legível (Ex: A0001) antes de gravar
            arranjo.Id = GenerateNextId(lista);

            // 2. Adicionar à memória
            lista.Add(arranjo);

            // 3. Persistir no disco
            SaveChanges(lista);
        }

        public void Update(Arranjo arranjoAtualizado)
        {
            var lista = GetAllArranjos();

            // Como não temos SQL "UPDATE WHERE ID = X", temos de encontrar o objeto na lista
            int index = lista.FindIndex(a => a.Id == arranjoAtualizado.Id);

            if (index != -1)
            {
                // Substitui o objeto antigo pelo novo (com estado atualizado)
                lista[index] = arranjoAtualizado;

                // Reescreve o JSON todo
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

        // --- MÉTODOS AUXILIARES ---

        /// <summary>
        /// Gera IDs sequenciais alfanuméricos: A0001 -> ... -> A9999 -> B0001.
        /// Garante que os talões são únicos e fáceis de ler.
        /// </summary>
        private string GenerateNextId(List<Arranjo> lista)
        {
            // Se for o primeiro registo da loja
            if (lista == null || lista.Count == 0) return "A0001";

            var ultimo = lista.Last();
            string ultimoId = ultimo.Id; // Ex: "A0015"

            // Validação de segurança para formatos inválidos
            if (string.IsNullOrEmpty(ultimoId) || ultimoId.Length < 2) return "A0001";

            char letra = ultimoId[0];
            string parteNumerica = ultimoId.Substring(1);

            if (int.TryParse(parteNumerica, out int numero))
            {
                numero++;

                // Se esgotarmos os números (9999), avançamos a letra (A -> B) e resetamos o contador
                if (numero > 9999)
                {
                    numero = 1;
                    letra++;
                }

                // Formata com zeros à esquerda (PadLeft)
                return $"{letra}{numero:D4}";
            }

            return "ERR00"; // Fallback caso o ID anterior esteja corrompido
        }
    }
}