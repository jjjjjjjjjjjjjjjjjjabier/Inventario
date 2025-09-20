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
    public partial class TiposEquipoViewModel : BaseViewModel
    {
        private readonly ITipoEquipoService _srv;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private TipoEquipo? _tipoEquipoSeleccionado;

        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();

        public TiposEquipoViewModel(ITipoEquipoService srv, IDialogService dialogService, ILogger<TiposEquipoViewModel> log)
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
                TiposEquipo.Clear();
                var lista = await _srv.BuscarAsync(null, true);
                foreach (var item in lista) TiposEquipo.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al buscar tipos de equipo.");
                ShowError("Ocurrió un error al cargar los tipos de equipo.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CrearAsync()
        {
            var nuevo = new TipoEquipo { Activo = true };
            if (_dialogService.ShowDialog<TipoEquipoEditorViewModel>(vm => vm.SetEntidad(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool CanEditarEliminar() => TipoEquipoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (TipoEquipoSeleccionado == null) return;
            if (_dialogService.ShowDialog<TipoEquipoEditorViewModel>(vm => vm.SetEntidad(TipoEquipoSeleccionado)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (TipoEquipoSeleccionado == null) return;
            if (!ConfirmAction($"¿Eliminar el tipo de equipo '{TipoEquipoSeleccionado.Nombre}'?", "Confirmar Eliminación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(TipoEquipoSeleccionado.Id);
                TiposEquipo.Remove(TipoEquipoSeleccionado);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar tipo de equipo.");
                ShowError("No se pudo eliminar. Es posible que esté en uso por algún equipo.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}