using System.Windows;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProjectTests.Fakes
{
    /// <summary>
    /// Fábrica de Janelas "Muda".
    /// Nos testes unitários, não queremos que janelas reais abram (o que bloquearia a execução).
    /// Esta classe finge que abre janelas, mas não faz nada, permitindo testar a lógica de navegação.
    /// </summary>
    public class FakeViewFactory : IViewFactory
    {
        public Window ShowDialog(ViewType type, object? parameter = null)
        {
            // Retorna null ou uma janela vazia. 
            // O teste apenas verifica se o método foi chamado, não precisa de ver a janela.
            return null;
        }
    }
}