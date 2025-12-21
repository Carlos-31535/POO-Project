using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;

namespace POOProject.Models.Repositories
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly string _filePath;

        public FuncionarioRepository()
        {
            // Define o caminho relativo para a pasta "Data" junto ao executável.
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            // Cria a diretoria se não existir para evitar erros de 'DirectoryNotFound'.
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            _filePath = Path.Combine(folder, "funcionarios.json");

            // Inicializa o ficheiro com um array vazio se for a primeira execução
            if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        }

        public Funcionario GetByUsername(string username)
        {
            var lista = GetAll();
            // StringComparison.OrdinalIgnoreCase ignora maiúsculas/minúsculas (Admin == admin)
            return lista.FirstOrDefault(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public void SaveOrUpdate(Funcionario funcionario)
        {
            var lista = GetAll();

            var existente = lista.FirstOrDefault(f => f.Username == funcionario.Username);

            if (existente != null)
            {
                // MODO EDIÇÃO: Atualizamos os campos permitidos
                existente.AtualizarDados(funcionario.FirstName, funcionario.LastName);
            }
            else
            {
                // MODO CRIAÇÃO:
                // Simulação de "Auto-Increment" de base de dados.
                // Pega no último ID e soma 1. Se a lista estiver vazia, começa no 1.
                int proximoId = lista.Any() ? lista.Max(f => f.Id) + 1 : 1;
                funcionario.Id = proximoId;

                lista.Add(funcionario);
            }

            // Regrava o ficheiro completo com as alterações
            SaveToFile(lista);
        }

        public List<Funcionario> GetAll()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<Funcionario>();
                string json = File.ReadAllText(_filePath);

                // O '??' garante que nunca devolvemos null, evitando crashes na UI
                return JsonSerializer.Deserialize<List<Funcionario>>(json) ?? new List<Funcionario>();
            }
            catch
            {
                // Se o ficheiro estiver corrompido, devolve lista vazia para o programa não fechar.
                return new List<Funcionario>();
            }
        }

        private void SaveToFile(List<Funcionario> lista)
        {
            // WriteIndented deixa o JSON bonito e legível se abrirmos no Bloco de Notas
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(lista, options);
            File.WriteAllText(_filePath, json);
        }
    }
}