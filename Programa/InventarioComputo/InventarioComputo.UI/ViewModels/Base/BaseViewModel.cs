using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace InventarioComputo.UI.ViewModels.Base
{
    // Hacemos que la clase sea 'partial' y herede de 'ObservableObject'
    // que ya implementa INotifyPropertyChanged de forma optimizada.
    public abstract partial class BaseViewModel : ObservableObject
    {
        // El atributo [ObservableProperty] crea automáticamente una propiedad pública
        // llamada "IsBusy" y se encarga de notificar los cambios.
        [ObservableProperty]
        private bool _isBusy;

        // Propiedad para el logger que podrán usar todos los ViewModels hijos.
        protected ILogger? Logger { get; set; }

        protected void ShowError(string mensaje)
            => MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        protected void ShowInfo(string mensaje)
            => MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);

        protected bool ConfirmAction(string mensaje, string titulo)
            => MessageBox.Show(mensaje, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}