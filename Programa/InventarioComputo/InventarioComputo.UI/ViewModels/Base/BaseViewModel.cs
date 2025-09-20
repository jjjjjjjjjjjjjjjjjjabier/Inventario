using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

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

        // **CAMBIO CLAVE:** Se han eliminado los métodos ShowError, ShowInfo y ConfirmAction.
        // Cada ViewModel que necesite mostrar mensajes ahora deberá inyectar y usar IDialogService.
    }
}