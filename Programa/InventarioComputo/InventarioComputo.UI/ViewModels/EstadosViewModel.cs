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
    public partial class EstadosViewModel : BaseViewModel
    {
        private readonly IEstadoService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Estado? _estadoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        private bool _esAdministrador;

        public ObservableCollection<Estado> Estados { get; } = new();

        public EstadosViewModel(
            IEstadoService srv,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<EstadosViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = log;

            EsAdministrador = _sessionService.TieneRol("Administrador");
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        public async Task CrearAsync()
        {
            var nuevo = new Estado { Activo = true, ColorHex = "#FFFFFF" };
            if (_dialogService.ShowDialog<EstadoEditorViewModel>(vm => vm.SetEstado(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && EstadoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        public async Task EditarAsync()
        {
            if (EstadoSeleccionado == null) return;

            // Clonar para evitar editar la instancia mostrada en la grilla
            var copia = new Estado
            {
                Id = EstadoSeleccionado.Id,
                Nombre = EstadoSeleccionado.Nombre,
                Descripcion = EstadoSeleccionado.Descripcion,
                ColorHex = EstadoSeleccionado.ColorHex,
                Activo = EstadoSeleccionado.Activo
            };

            if (_dialogService.ShowDialog<EstadoEditorViewModel>(vm => vm.SetEstado(copia)) == true)
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
                Estados.Clear();
                var lista = await _srv.BuscarAsync(null, MostrarInactivos);
                foreach (var item in lista) Estados.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando estados");
                _dialogService.ShowError("Ocurrió un error al cargar los estados.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (EstadoSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Está seguro de querer desactivar el estado '{EstadoSeleccionado.Nombre}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(EstadoSeleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar estado");
                _dialogService.ShowError("No se pudo desactivar el estado. Es posible que esté en uso.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();
    }
}