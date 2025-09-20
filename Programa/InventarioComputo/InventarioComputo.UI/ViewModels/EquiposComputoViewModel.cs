using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class EquiposComputoViewModel : BaseViewModel
    {
        private readonly IEquipoComputoService _srv;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private EquipoComputo? _equipoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        public ObservableCollection<EquipoComputo> Equipos { get; } = new();

        public EquiposComputoViewModel(IEquipoComputoService srv, IDialogService dialogService, ILogger<EquiposComputoViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();

        [RelayCommand]
        private async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Equipos.Clear();
                var lista = await _srv.ObtenerTodosAsync(MostrarInactivos);
                foreach (var item in lista) Equipos.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando equipos.");
                _dialogService.ShowError("Error al cargar los equipos de cómputo.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CrearAsync()
        {
            var nuevo = new EquipoComputo();
            if (_dialogService.ShowDialog<EquipoComputoEditorViewModel>(vm => vm.SetEquipo(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool CanEditDelete() => EquipoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditDelete))]
        private async Task EditarAsync()
        {
            if (EquipoSeleccionado == null) return;
            var equipoAEditar = await _srv.ObtenerPorIdAsync(EquipoSeleccionado.Id);
            if (equipoAEditar == null)
            {
                _dialogService.ShowError("No se encontró el equipo. La lista se refrescará.");
                await BuscarAsync();
                return;
            }
            if (_dialogService.ShowDialog<EquipoComputoEditorViewModel>(vm => vm.SetEquipo(equipoAEditar)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDelete))]
        private async Task EliminarAsync()
        {
            if (EquipoSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Está seguro de querer desactivar el equipo con número de serie '{EquipoSeleccionado.NumeroSerie}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(EquipoSeleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar equipo.");
                _dialogService.ShowError("Ocurrió un error al desactivar el equipo.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}