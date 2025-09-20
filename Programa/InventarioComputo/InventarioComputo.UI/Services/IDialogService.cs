// Archivo: InventarioComputo.UI/Services/IDialogService.cs

using System;

namespace InventarioComputo.UI.Services
{
    // Interfaz auxiliar para identificar ViewModels que son para editores.
    public interface IEditorViewModel { }

    public interface IDialogService
    {
        // Un único método genérico para mostrar cualquier diálogo.
        // Devuelve 'bool?' para saber si el usuario guardó (true) o canceló (false/null).
        bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState) where TViewModel : class, IEditorViewModel;
    }
}