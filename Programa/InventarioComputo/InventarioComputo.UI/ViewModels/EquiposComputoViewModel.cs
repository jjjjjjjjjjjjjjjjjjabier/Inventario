using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class EquiposComputoViewModel : BaseViewModel, IDisposable
    {
        private readonly IEquipoComputoService _srv;
        private readonly IMovimientoService _movimientoSvc;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        private CancellationTokenSource? _buscarCts;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        [NotifyCanExecuteChangedFor(nameof(VerHistorialCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsignarEquipoCommand))]
        private EquipoComputo? _equipoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsignarEquipoCommand))]
        private bool _esAdministrador;

        private string _filtroTexto = string.Empty;
        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                if (SetProperty(ref _filtroTexto, value))
                {
                    _ = BuscarAsyncDebounced();
                }
            }
        }

        public ObservableCollection<EquipoComputo> Equipos { get; } = new();

        public EquiposComputoViewModel(
            IEquipoComputoService srv,
            IMovimientoService movimientoSvc,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<EquiposComputoViewModel> log)
        {
            _srv = srv;
            _movimientoSvc = movimientoSvc;
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = log;

            EsAdministrador = _sessionService.TieneRol("Administrador");
            _sessionService.SesionCambiada += OnSesionCambiada;
        }

        private void OnSesionCambiada(object? sender, bool autenticado)
        {
            EsAdministrador = _sessionService.TieneRol("Administrador");
            CrearCommand?.NotifyCanExecuteChanged();
            EditarCommand?.NotifyCanExecuteChanged();
            EliminarCommand?.NotifyCanExecuteChanged();
            VerHistorialCommand?.NotifyCanExecuteChanged();
            AsignarEquipoCommand?.NotifyCanExecuteChanged();
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
        private bool CanEditarEliminar() => EsAdministrador && _equipoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        public async Task EditarAsync()
        {
            if (_equipoSeleccionado == null) return;
            if (_dialogService.ShowDialog<EquipoComputoEditorViewModel>(vm => vm.SetEquipo(_equipoSeleccionado)) == true)
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
                var filtro = string.IsNullOrWhiteSpace(FiltroTexto) ? null : FiltroTexto.Trim();
                var lista = await _srv.BuscarAsync(filtro, MostrarInactivos);
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
                EditarCommand?.NotifyCanExecuteChanged();
                EliminarCommand?.NotifyCanExecuteChanged();
                VerHistorialCommand?.NotifyCanExecuteChanged();
                AsignarEquipoCommand?.NotifyCanExecuteChanged();
            }
        }

        private async Task BuscarAsyncDebounced(int delayMs = 300)
        {
            try
            {
                _buscarCts?.Cancel();
                _buscarCts?.Dispose();
                _buscarCts = new CancellationTokenSource();
                var ct = _buscarCts.Token;

                await Task.Delay(delayMs, ct);
                if (ct.IsCancellationRequested) return;

                await BuscarAsync();
            }
            catch (TaskCanceledException) { }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (_equipoSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Está seguro de querer desactivar el equipo con número de serie '{_equipoSeleccionado.NumeroSerie}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(_equipoSeleccionado.Id);
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

        private bool CanVerHistorial() => _equipoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanVerHistorial))]
        public async Task VerHistorial()
        {
            if (_equipoSeleccionado == null) return;

            await Task.Yield();
            _dialogService.ShowDialog<HistorialEquipoViewModel>(vm =>
            {
                if (vm is HistorialEquipoViewModel historialVM)
                {
                    _ = historialVM.CargarHistorialAsync(_equipoSeleccionado.Id);
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        public async Task AsignarEquipo()
        {
            if (_equipoSeleccionado == null) return;

            try
            {
                var equipo = await _srv.ObtenerPorIdAsync(_equipoSeleccionado.Id);
                if (equipo != null)
                {
                    _dialogService.ShowDialog<AsignarEquipoViewModel>(vm =>
                    {
                        _ = vm.InitializeAsync(equipo);
                    });
                    await BuscarAsync();
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al asignar equipo");
                _dialogService.ShowError("Ocurrió un error al asignar el equipo.");
            }
        }

        public void Dispose()
        {
            _sessionService.SesionCambiada -= OnSesionCambiada;
            _buscarCts?.Cancel();
            _buscarCts?.Dispose();
        }
    }
}