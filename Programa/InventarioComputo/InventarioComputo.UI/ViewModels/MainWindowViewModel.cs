using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InventarioComputo.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        // CORRECCIÓN: Renombramos la propiedad a "CurrentViewModel" para que coincida con el XAML.
        [ObservableProperty]
        private BaseViewModel? _currentViewModel;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // La aplicación iniciará en la vista de Equipos de Cómputo por defecto.
            NavigateToEquiposComputoCommand.Execute(null);
        }

        // CORRECCIÓN: Renombramos los métodos para mayor claridad (NavigateTo...).
        [RelayCommand]
        private void NavigateToUbicaciones()
            => CurrentViewModel = _serviceProvider.GetRequiredService<UbicacionesViewModel>();

        [RelayCommand]
        private void NavigateToUnidades()
            => CurrentViewModel = _serviceProvider.GetRequiredService<UnidadesViewModel>();

        [RelayCommand]
        private void NavigateToEstados()
            => CurrentViewModel = _serviceProvider.GetRequiredService<EstadosViewModel>();

        [RelayCommand]
        private void NavigateToTiposEquipo()
            => CurrentViewModel = _serviceProvider.GetRequiredService<TiposEquipoViewModel>();

        [RelayCommand]
        private void NavigateToEquiposComputo()
            => CurrentViewModel = _serviceProvider.GetRequiredService<EquiposComputoViewModel>();
    }
}