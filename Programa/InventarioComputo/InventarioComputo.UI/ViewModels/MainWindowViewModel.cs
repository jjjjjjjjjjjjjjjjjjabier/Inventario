using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.UI.ViewModels.Base;

namespace InventarioComputo.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        // ViewModels para cada sección de la aplicación, inyectados por DI.
        private readonly EquiposComputoViewModel _equiposComputoViewModel;
        private readonly EstadosViewModel _estadosViewModel;
        private readonly UnidadesViewModel _unidadesViewModel;
        private readonly UbicacionesViewModel _ubicacionesViewModel;
        private readonly TiposEquipoViewModel _tiposEquipoViewModel;

        [ObservableProperty]
        private BaseViewModel? _currentViewModel;

        public MainWindowViewModel(
            EquiposComputoViewModel equiposComputoViewModel,
            EstadosViewModel estadosViewModel,
            UnidadesViewModel unidadesViewModel,
            UbicacionesViewModel ubicacionesViewModel,
            TiposEquipoViewModel tiposEquipoViewModel)
        {
            _equiposComputoViewModel = equiposComputoViewModel;
            _estadosViewModel = estadosViewModel;
            _unidadesViewModel = unidadesViewModel;
            _ubicacionesViewModel = ubicacionesViewModel;
            _tiposEquipoViewModel = tiposEquipoViewModel;

            // Establecer la vista inicial al arrancar la aplicación.
            CurrentViewModel = _equiposComputoViewModel;
        }

        [RelayCommand]
        private void NavigateToEquiposComputo() => CurrentViewModel = _equiposComputoViewModel;

        [RelayCommand]
        private void NavigateToEstados() => CurrentViewModel = _estadosViewModel;

        [RelayCommand]
        private void NavigateToUnidades() => CurrentViewModel = _unidadesViewModel;

        [RelayCommand]
        private void NavigateToUbicaciones() => CurrentViewModel = _ubicacionesViewModel;

        [RelayCommand]
        private void NavigateToTiposEquipo() => CurrentViewModel = _tiposEquipoViewModel;
    }
}