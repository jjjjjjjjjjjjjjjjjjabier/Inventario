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
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private EquipoComputo? _equipoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        private bool _esAdministrador;

        public ObservableCollection<EquipoComputo> Equipos { get; } = new();

        public EquiposComputoViewModel(
            IEquipoComputoService srv, 
            IDialogService dialogService, 
            ISessionService sessionService,
            ILogger<EquiposComputoViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = log;
            
            // Verificar permisos del usuario
            EsAdministrador = _sessionService.TieneRol("Administrador");
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        public async Task CrearAsync()
        {
            var nuevo = new EquipoComputo { Activo = true, FechaAdquisicion = DateTime.Today };
            if (_dialogService.ShowDialog<EquipoComputoEditorViewModel>(vm => vm.SetEquipo(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && EquipoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        public async Task EditarAsync()
        {
            if (EquipoSeleccionado == null) return;
            if (_dialogService.ShowDialog<EquipoComputoEditorViewModel>(vm => vm.SetEquipo(EquipoSeleccionado)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand]
        public async Task BuscarAsync()
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
                Logger?.LogError(ex, "Error buscando equipos");
                _dialogService.ShowError("Ocurrió un error al cargar los equipos.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
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

        [RelayCommand(CanExecute = nameof(CanVerHistorial))]
        public void VerHistorial()
        {
            if (EquipoSeleccionado == null) return;
            _dialogService.ShowDialog<HistorialEquipoViewModel>(vm => vm.CargarHistorialAsync(EquipoSeleccionado.Id));
        }

        private bool CanVerHistorial() => EquipoSeleccionado != null && !IsBusy;
    }
}