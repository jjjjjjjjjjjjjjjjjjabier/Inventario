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
    public partial class TiposEquipoViewModel : BaseViewModel, IDisposable
    {
        private readonly ITipoEquipoService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private TipoEquipo? _tipoEquipoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        private string _filtro = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private bool _esAdministrador;

        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();

        public TiposEquipoViewModel(
            ITipoEquipoService srv,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<TiposEquipoViewModel> log)
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

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();
        partial void OnFiltroChanged(string value) => _ = BuscarAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            SetBusy(true);
            try
            {
                TiposEquipo.Clear();
                var filtro = string.IsNullOrWhiteSpace(Filtro) ? null : Filtro.Trim();
                var lista = await _srv.BuscarAsync(filtro, MostrarInactivos);
                foreach (var item in lista) TiposEquipo.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando tipos de equipo");
                _dialogService.ShowError("Ocurrió un error al cargar los tipos de equipo.");
            }
            finally
            {
                SetBusy(false);
            }
        }

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        private async Task CrearAsync()
        {
            var nuevo = new TipoEquipo { Activo = true };
            if (_dialogService.ShowDialog<TipoEquipoEditorViewModel>(vm => vm.SetEntidad(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && TipoEquipoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (TipoEquipoSeleccionado == null) return;

            var copia = new TipoEquipo
            {
                Id = TipoEquipoSeleccionado.Id,
                Nombre = TipoEquipoSeleccionado.Nombre,
                Activo = TipoEquipoSeleccionado.Activo
            };

            if (_dialogService.ShowDialog<TipoEquipoEditorViewModel>(vm => vm.SetEntidad(copia)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (TipoEquipoSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Desactivar el tipo de equipo '{TipoEquipoSeleccionado.Nombre}'?", "Confirmar Desactivación")) return;

            SetBusy(true);
            try
            {
                await _srv.EliminarAsync(TipoEquipoSeleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar tipo de equipo");
                _dialogService.ShowError("No se pudo desactivar el tipo de equipo. Es posible que esté en uso.");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool value)
        {
            IsBusy = value;
            ActualizarCanExecute();
        }

        private void ActualizarCanExecute()
        {
            CrearCommand?.NotifyCanExecuteChanged();
            EditarCommand?.NotifyCanExecuteChanged();
            EliminarCommand?.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _sessionService.SesionCambiada -= OnSesionCambiada;
        }
    }
}