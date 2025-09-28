using InventarioComputo.UI.ViewModels.Base;
using System;
using System.Windows;

namespace InventarioComputo.UI.Services
{
    public interface IDialogService
    {
        // Método para registrar explícitamente ViewModels con sus Views
        void Register<TViewModel, TView>() where TViewModel : BaseViewModel 
                                          where TView : Window;
        
        // Método para mostrar diálogos
        bool? ShowDialog<TViewModel>(Action<TViewModel> setViewModelState) where TViewModel : BaseViewModel;

        // Métodos de mensajes
        void ShowInfo(string message);
        void ShowError(string message);
        bool Confirm(string message, string title);
    }
}