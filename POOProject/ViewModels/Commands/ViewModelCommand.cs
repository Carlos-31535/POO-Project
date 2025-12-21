using System;
using System.Windows.Input;

namespace POOProject.ViewModels.Commands
{
    /// <summary>
    /// Classe genérica que implementa ICommand.
    /// É a peça fundamental do MVVM que permite ligar botões da View a métodos do ViewModel
    /// sem usar eventos "Click" no code-behind.
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        // --- DELEGATES ---
        // Guardam as funções que devem ser executadas quando o botão é clicado.

        // Ação principal (o que o comando faz)
        public delegate void ICommandOnExecute(object? parameter);

        // Validação (se o botão deve estar ativo ou cinzento/disabled)
        public delegate bool ICommandOnCanExecute(object? parameter);

        // --- CAMPOS ---
        private readonly ICommandOnExecute _execute;
        private readonly ICommandOnCanExecute? _canExecute;

        // --- CONSTRUTORES ---

        // Construtor simples: O comando está sempre disponível para executar
        public ViewModelCommand(ICommandOnExecute onExecuteMethod)
        {
            _execute = onExecuteMethod ?? throw new ArgumentNullException(nameof(onExecuteMethod));
        }

        // Construtor completo: Inclui lógica para ativar/desativar o botão
        public ViewModelCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
        {
            _execute = onExecuteMethod ?? throw new ArgumentNullException(nameof(onExecuteMethod));
            _canExecute = onCanExecuteMethod ?? throw new ArgumentNullException(nameof(onCanExecuteMethod));
        }

        // --- INTERFACE ICOMMAND ---

        // Evento mágico do WPF: avisa a interface gráfica para reavaliar se o botão deve estar ativo.
        // O CommandManager.RequerySuggested faz isso automaticamente quando o utilizador mexe na UI.
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Pergunta ao ViewModel: "Posso correr agora?" (Ex: O botão Login só ativa se tiver texto nas caixas)
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        // Executa a lógica real
        public void Execute(object? parameter)
        {
            _execute.Invoke(parameter);
        }
    }
}