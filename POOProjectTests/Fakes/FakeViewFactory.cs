using System.Windows;
using POOProject.Views.Enums;
using POOProject.Views.Interfaces;

namespace POOProjectTests.Fakes
{
    public class FakeViewFactory : IViewFactory
    {
        public Window ShowDialog(ViewType type, object? parameter = null)
        {
            // Retorna null. O teste não precisa de ver a janela real.
            return null;
        }
    }
}