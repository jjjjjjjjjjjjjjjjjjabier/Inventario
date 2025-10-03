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
    public partial class EmpleadosViewModel : BaseViewModel, IDisposable
    {
        private readonly IEmpleadoService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;
        private CancellationTokenSource? _buscarCts;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Empleado? _empleadoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
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

        public ObservableCollection<Empleado> Empleados { get; } = new();

        public EmpleadosViewModel(
            IEmpleadoService srv,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<EmpleadosViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = log;

            EsAdministrador = _sessionService.TieneRol("Administrador");
            _sessionService.SesionCambiada += OnSesionCambiada;
        }

        private void OnSesionCambiada(object? sender, bool estaLogueado)
        {
            EsAdministrador = _sessionService.TieneRol("Administrador");
            ActualizarCanExecute();
        }

        private void ActualizarCanExecute()
        {
            CrearCommand?.NotifyCanExecuteChanged();
            EditarCommand?.NotifyCanExecuteChanged();
            EliminarCommand?.NotifyCanExecuteChanged();
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Empleados.Clear();
                var filtro = string.IsNullOrWhiteSpace(FiltroTexto) ? null : FiltroTexto.Trim();
                var lista = await _srv.BuscarAsync(filtro, MostrarInactivos);
                foreach (var item in lista) Empleados.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando empleados");
                _dialogService.ShowError("Ocurrió un error al cargar los empleados.");
            }
            finally
            {
                IsBusy = false;
                ActualizarCanExecute();
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
            catch (TaskCanceledException) { /* esperado */ }
        }

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        private async Task CrearAsync()
        {
            var nuevo = new Empleado { Activo = true };
            if (_dialogService.ShowDialog<EmpleadoEditorViewModel>(vm => vm.SetEmpleado(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && EmpleadoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (EmpleadoSeleccionado == null) return;

            var copia = new Empleado
            {
                Id = EmpleadoSeleccionado.Id,
                NombreCompleto = EmpleadoSeleccionado.NombreCompleto,
                Puesto = EmpleadoSeleccionado.Puesto,
                Activo = EmpleadoSeleccionado.Activo
            };

            if (_dialogService.ShowDialog<EmpleadoEditorViewModel>(vm => vm.SetEmpleado(copia)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (EmpleadoSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Desactivar el empleado '{EmpleadoSeleccionado.NombreCompleto}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(EmpleadoSeleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar empleado");
                _dialogService.ShowError("No se pudo desactivar el empleado. Es posible que tenga equipos asignados.");
            }
            finally
            {
                IsBusy = false;
                ActualizarCanExecute();
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