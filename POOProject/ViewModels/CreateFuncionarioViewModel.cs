using POOProject.Models.Entities;
using POOProject.Models.Repositories.Interfaces;
using POOProject.ViewModels.Commands;
using System;
using System.Windows;
using System.Windows.Input;

namespace POOProject.ViewModels
{
    public class CreateFuncionarioViewModel : BaseViewModel
    {
        private readonly IFuncionarioRepository _repository;

        // Propriedades do Formulário
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(nameof(LastName)); }
        }

        // Ação para fechar a janela
        public Action? HideWindowAction { get; set; }

        // Comando do Botão
        public ICommand SaveCommand { get; }

        // Construtor
        public CreateFuncionarioViewModel(IFuncionarioRepository repository)
        {
            _repository = repository;
            SaveCommand = new ViewModelCommand(ExecuteSave);
        }

        private void ExecuteSave(object? obj)
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("Preencha o nome e apelido.");
                return;
            }

            // Cria o novo funcionário (ID é gerado no repositório)
            var novoFuncionario = new Funcionario(FirstName, LastName, 0)
            {
                Username = FirstName.ToLower() // Gera um username simples
            };

            // Guarda na base de dados
            _repository.SaveOrUpdate(novoFuncionario);

            MessageBox.Show("Funcionário adicionado com sucesso!");

            // Fecha a janela
            HideWindowAction?.Invoke();
        }
    }
}