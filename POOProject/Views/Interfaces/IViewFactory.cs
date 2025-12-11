
using System.Windows;
using POOProject.Views.Enums;

namespace POOProject.Views.Interfaces
{
    /// <summary>
    /// Defines a factory for creating views/windows.
    /// </summary>
    public interface IViewFactory
    {
        #region Methods

        /// <summary>
        /// Creates a <see cref="Window"/> instance of the specified <see cref="ViewType"/>.
        /// </summary>
        /// <param name="type">The type of view to create.</param>
        /// <param name="parameter">Optional parameter to pass to the view.</param>
        /// <returns>A new instance of a <see cref="Window"/> corresponding to the specified view type.</returns>
        Window ShowDialog(ViewType type, object? parameter = null);

        #endregion
    }
}
