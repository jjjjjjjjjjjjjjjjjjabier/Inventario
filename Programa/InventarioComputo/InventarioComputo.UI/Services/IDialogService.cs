using InventarioComputo.UI.ViewModels.Base;
using System;

namespace InventarioComputo.UI.Services
{
    public interface IDialogService
    {
        // Este método ya lo tenías, para abrir ventanas de edición.
        bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState) where TViewModel : BaseViewModel;

        // --- NUEVOS MÉTODOS ---

        /// <summary>
        /// Muestra un mensaje de información al usuario.
        /// </summary>
        /// <param name="message">El texto a mostrar.</param>
        void ShowInfo(string message);

        /// <summary>
        /// Muestra un mensaje de error al usuario.
        /// </summary>
        /// <param name="message">El texto del error a mostrar.</param>
        void ShowError(string message);

        /// <summary>
        /// Muestra un cuadro de diálogo de confirmación (Sí/No) al usuario.
        /// </summary>
        /// <param name="message">La pregunta de confirmación.</param>
        /// <param name="title">El título de la ventana.</param>
        /// <returns>True si el usuario presiona "Sí", de lo contrario False.</returns>
        bool Confirm(string message, string title);
    }
}