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
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            _filePath = Path.Combine(folder, "funcionarios.json");

            if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        }

        public Funcionario GetByUsername(string username)
        {
            var lista = GetAll();
            return lista.FirstOrDefault(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public void SaveOrUpdate(Funcionario funcionario)
        {
            var lista = GetAll();

            var existente = lista.FirstOrDefault(f => f.Username == funcionario.Username);

            if (existente != null)
            {
                // [CORREÇÃO AQUI] Usa o método público para atualizar
                existente.AtualizarDados(funcionario.FirstName, funcionario.LastName);
            }
            else
            {
                // É novo? Gera o próximo ID numérico
                int proximoId = lista.Any() ? lista.Max(f => f.Id) + 1 : 1;
                funcionario.Id = proximoId;

                lista.Add(funcionario);
            }

            SaveToFile(lista);
        }

        public List<Funcionario> GetAll()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<Funcionario>();
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Funcionario>>(json) ?? new List<Funcionario>();
            }
            catch { return new List<Funcionario>(); }
        }

        private void SaveToFile(List<Funcionario> lista)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(lista, options);
            File.WriteAllText(_filePath, json);
        }
    }
}