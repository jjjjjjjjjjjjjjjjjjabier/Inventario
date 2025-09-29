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
    public partial class UnidadesViewModel : BaseViewModel, IDisposable
    {
        private readonly IUnidadService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Unidad? _unidadSeleccionada;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private bool _esAdministrador;

        public ObservableCollection<Unidad> Unidades { get; } = new();

        public UnidadesViewModel(
            IUnidadService srv,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<UnidadesViewModel> log)
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
                Unidades.Clear();
                var lista = await _srv.BuscarAsync(null, MostrarInactivos);
                foreach (var item in lista) Unidades.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando unidades");
                _dialogService.ShowError("Ocurrió un error al cargar las unidades.");
            }
            finally
            {
                IsBusy = false;
                ActualizarCanExecute();
            }
        }

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        private async Task CrearAsync()
        {
            var nuevo = new Unidad { Activo = true };
            if (_dialogService.ShowDialog<UnidadEditorViewModel>(vm => vm.SetEntidad(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && UnidadSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (UnidadSeleccionada == null) return;

            var copia = new Unidad
            {
                Id = UnidadSeleccionada.Id,
                Nombre = UnidadSeleccionada.Nombre,
                Abreviatura = UnidadSeleccionada.Abreviatura,
                Activo = UnidadSeleccionada.Activo
            };

            if (_dialogService.ShowDialog<UnidadEditorViewModel>(vm => vm.SetEntidad(copia)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (UnidadSeleccionada == null) return;
            if (!_dialogService.Confirm($"¿Desactivar la unidad '{UnidadSeleccionada.Nombre}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(UnidadSeleccionada.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar unidad");
                _dialogService.ShowError("No se pudo desactivar la unidad. Es posible que esté en uso.");
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
        }
    }
}