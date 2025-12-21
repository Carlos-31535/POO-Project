using System.Windows;
using POOProject.Views.Enums;

namespace POOProject.Views.Interfaces
{
    /// <summary>
    /// Contrato para a criação de janelas (Factory Pattern).
    /// Permite que os ViewModels peçam para abrir novas janelas sem saberem
    /// que estão numa aplicação WPF (desacoplamento).
    /// </summary>
    public interface IViewFactory
    {
        // Método único para abrir qualquer janela do sistema.
        // Recebe o 'tipo' da janela (Enum) em vez da classe concreta.
        Window ShowDialog(ViewType type, object? parameter = null);
    }
}