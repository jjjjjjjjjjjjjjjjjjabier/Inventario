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
    public partial class UbicacionesViewModel : BaseViewModel, IDisposable
    {
        private readonly ISedeService _sedeSvc;
        private readonly IAreaService _areaSvc;
        private readonly IZonaService _zonaSvc;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearAreaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarSedeCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarSedeCommand))]
        private Sede? _sedeSeleccionada;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearZonaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarAreaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarAreaCommand))]
        private Area? _areaSeleccionada;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarZonaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarZonaCommand))]
        private Zona? _zonaSeleccionada;

        [ObservableProperty]
        private bool _mostrarInactivas;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearSedeCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarSedeCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarSedeCommand))]
        [NotifyCanExecuteChangedFor(nameof(CrearAreaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarAreaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarAreaCommand))]
        [NotifyCanExecuteChangedFor(nameof(CrearZonaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarZonaCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarZonaCommand))]
        private bool _esAdministrador;

        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        public UbicacionesViewModel(
            ISedeService sedeSvc,
            IAreaService areaSvc,
            IZonaService zonaSvc,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<UbicacionesViewModel> log)
        {
            _sedeSvc = sedeSvc;
            _areaSvc = areaSvc;
            _zonaSvc = zonaSvc;
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

        partial void OnSedeSeleccionadaChanged(Sede? value) => _ = BuscarAreasAsync();
        partial void OnAreaSeleccionadaChanged(Area? value) => _ = BuscarZonasAsync();
        partial void OnMostrarInactivasChanged(bool value) => _ = BuscarSedesAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarSedesAsync();

        // --- SEDES ---
        [RelayCommand]
        private async Task BuscarSedesAsync()
        {
            SetBusy(true);
            try
            {
                Sedes.Clear();
                Areas.Clear();
                Zonas.Clear();
                var sedes = await _sedeSvc.BuscarAsync(null, MostrarInactivas);
                foreach (var s in sedes) Sedes.Add(s);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando sedes");
                _dialogService.ShowError("Error al cargar sedes.");
            }
            finally { SetBusy(false); }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;
        private bool CanEditDeleteSede() => EsAdministrador && SedeSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        public async Task CrearSedeAsync()
        {
            var nueva = new Sede { Activo = true };
            if (_dialogService.ShowDialog<SedeEditorViewModel>(vm => vm.SetEntidad(nueva)) == true)
            {
                await BuscarSedesAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteSede))]
        public async Task EditarSedeAsync()
        {
            if (SedeSeleccionada == null) return;

            var copia = new Sede
            {
                Id = SedeSeleccionada.Id,
                Nombre = SedeSeleccionada.Nombre,
                Activo = SedeSeleccionada.Activo
            };

            if (_dialogService.ShowDialog<SedeEditorViewModel>(vm => vm.SetEntidad(copia)) == true)
            {
                await BuscarSedesAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteSede))]
        public async Task EliminarSedeAsync()
        {
            if (SedeSeleccionada == null) return;
            if (!_dialogService.Confirm($"¿Está seguro de querer desactivar la sede '{SedeSeleccionada.Nombre}'? Sus áreas y zonas asociadas no serán visibles.", "Confirmar Desactivación"))
                return;

            SetBusy(true);
            try
            {
                await _sedeSvc.EliminarAsync(SedeSeleccionada.Id);
                await BuscarSedesAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar sede");
                _dialogService.ShowError("No se pudo desactivar la sede. Es posible que esté en uso.");
            }
            finally { SetBusy(false); }
        }

        // --- AREAS ---
        private async Task BuscarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();
            if (SedeSeleccionada != null)
            {
                try
                {
                    var areas = await _areaSvc.BuscarAsync(SedeSeleccionada.Id, null, MostrarInactivas, default);
                    foreach (var a in areas) Areas.Add(a);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error buscando áreas");
                    _dialogService.ShowError("Error al cargar áreas.");
                }
            }
        }

        private bool CanEditDeleteArea() => EsAdministrador && AreaSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditDeleteSede))]
        public async Task CrearAreaAsync()
        {
            if (SedeSeleccionada == null) return;
            var nueva = new Area { Activo = true, SedeId = SedeSeleccionada.Id };
            if (_dialogService.ShowDialog<AreaEditorViewModel>(vm => vm.SetEntidad(nueva, SedeSeleccionada)) == true)
            {
                await BuscarAreasAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteArea))]
        public async Task EditarAreaAsync()
        {
            if (AreaSeleccionada == null || SedeSeleccionada == null) return;

            var copia = new Area
            {
                Id = AreaSeleccionada.Id,
                SedeId = AreaSeleccionada.SedeId,
                Nombre = AreaSeleccionada.Nombre,
                Activo = AreaSeleccionada.Activo
            };

            if (_dialogService.ShowDialog<AreaEditorViewModel>(vm => vm.SetEntidad(copia, SedeSeleccionada)) == true)
            {
                await BuscarAreasAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteArea))]
        public async Task EliminarAreaAsync()
        {
            if (AreaSeleccionada == null) return;
            if (!_dialogService.Confirm($"¿Desactivar el área '{AreaSeleccionada.Nombre}'?", "Confirmar"))
                return;

            SetBusy(true);
            try
            {
                await _areaSvc.EliminarAsync(AreaSeleccionada.Id, default);
                await BuscarAreasAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar área");
                _dialogService.ShowError("No se pudo desactivar el área. Es posible que esté en uso.");
            }
            finally { SetBusy(false); }
        }

        // --- ZONAS ---
        private async Task BuscarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada != null)
            {
                try
                {
                    var zonas = await _zonaSvc.BuscarAsync(AreaSeleccionada.Id, null, MostrarInactivas, default);
                    foreach (var z in zonas) Zonas.Add(z);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error buscando zonas");
                    _dialogService.ShowError("Error al cargar zonas.");
                }
            }
        }

        private bool CanEditDeleteZona() => EsAdministrador && ZonaSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditDeleteArea))]
        public async Task CrearZonaAsync()
        {
            if (AreaSeleccionada == null) return;
            var nueva = new Zona { Activo = true, AreaId = AreaSeleccionada.Id };
            if (_dialogService.ShowDialog<ZonaEditorViewModel>(vm => vm.SetEntidad(nueva, AreaSeleccionada)) == true)
            {
                await BuscarZonasAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteZona))]
        public async Task EditarZonaAsync()
        {
            if (ZonaSeleccionada == null || AreaSeleccionada == null) return;

            var copia = new Zona
            {
                Id = ZonaSeleccionada.Id,
                AreaId = ZonaSeleccionada.AreaId,
                Nombre = ZonaSeleccionada.Nombre,
                Activo = ZonaSeleccionada.Activo
            };

            if (_dialogService.ShowDialog<ZonaEditorViewModel>(vm => vm.SetEntidad(copia, AreaSeleccionada)) == true)
            {
                await BuscarZonasAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteZona))]
        public async Task EliminarZonaAsync()
        {
            if (ZonaSeleccionada == null) return;
            if (!_dialogService.Confirm($"¿Desactivar la zona '{ZonaSeleccionada.Nombre}'?", "Confirmar"))
                return;

            SetBusy(true);
            try
            {
                await _zonaSvc.EliminarAsync(ZonaSeleccionada.Id, default);
                await BuscarZonasAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar zona");
                _dialogService.ShowError("No se pudo desactivar la zona. Es posible que esté en uso.");
            }
            finally { SetBusy(false); }
        }

        private void SetBusy(bool value)
        {
            IsBusy = value;
            ActualizarCanExecute();
        }

        private void ActualizarCanExecute()
        {
            CrearSedeCommand?.NotifyCanExecuteChanged();
            EditarSedeCommand?.NotifyCanExecuteChanged();
            EliminarSedeCommand?.NotifyCanExecuteChanged();
            CrearAreaCommand?.NotifyCanExecuteChanged();
            EditarAreaCommand?.NotifyCanExecuteChanged();
            EliminarAreaCommand?.NotifyCanExecuteChanged();
            CrearZonaCommand?.NotifyCanExecuteChanged();
            EditarZonaCommand?.NotifyCanExecuteChanged();
            EliminarZonaCommand?.NotifyCanExecuteChanged();
        }

        public void Dispose()
        {
            _sessionService.SesionCambiada -= OnSesionCambiada;
        }
    }
}