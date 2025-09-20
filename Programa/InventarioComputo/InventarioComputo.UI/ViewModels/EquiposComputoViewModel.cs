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

        public ObservableCollection<EquipoComputo> Equipos { get; } = new();

        public EquiposComputoViewModel(IEquipoComputoService srv, IDialogService dialogService, ILogger<EquiposComputoViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        [RelayCommand]
        private async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Equipos.Clear();
                var lista = await _srv.ObtenerTodosAsync();
                foreach (var item in lista) Equipos.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando equipos.");
                ShowError("Error al cargar los equipos de cómputo.");
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
                ShowError("No se encontró el equipo. La lista se refrescará.");
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
            if (!ConfirmAction($"¿Eliminar el equipo '{EquipoSeleccionado.NumeroSerie}'?", "Confirmar")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(EquipoSeleccionado.Id);
                Equipos.Remove(EquipoSeleccionado);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar equipo.");
                ShowError("Ocurrió un error al eliminar el equipo.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}