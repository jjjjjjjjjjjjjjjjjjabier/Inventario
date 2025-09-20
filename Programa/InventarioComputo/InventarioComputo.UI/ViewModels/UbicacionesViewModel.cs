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
    public partial class UbicacionesViewModel : BaseViewModel
    {
        private readonly ISedeService _sedeSvc;
        private readonly IAreaService _areaSvc;
        private readonly IZonaService _zonaSvc;
        private readonly IDialogService _dialogService;

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

        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        public UbicacionesViewModel(ISedeService sedeSvc, IAreaService areaSvc, IZonaService zonaSvc, IDialogService dialogService, ILogger<UbicacionesViewModel> log)
        {
            _sedeSvc = sedeSvc;
            _areaSvc = areaSvc;
            _zonaSvc = zonaSvc;
            _dialogService = dialogService;
            Logger = log;
        }

        partial void OnSedeSeleccionadaChanged(Sede? value) => _ = BuscarAreasAsync();
        partial void OnAreaSeleccionadaChanged(Area? value) => _ = BuscarZonasAsync();

        [RelayCommand]
        private async Task LoadedAsync() => await BuscarSedesAsync();

        // --- SEDES ---
        [RelayCommand]
        private async Task BuscarSedesAsync()
        {
            IsBusy = true;
            try
            {
                Sedes.Clear();
                var sedes = await _sedeSvc.BuscarAsync(null, true);
                foreach (var s in sedes) Sedes.Add(s);
            }
            catch (Exception ex) { Logger?.LogError(ex, "Error buscando sedes"); ShowError("Error al cargar sedes."); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task CrearSedeAsync()
        {
            var nueva = new Sede { Activo = true };
            if (_dialogService.ShowDialog<SedeEditorViewModel>(vm => vm.SetEntidad(nueva)) == true)
            {
                await BuscarSedesAsync();
            }
        }

        private bool CanEditDeleteSede() => SedeSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditDeleteSede))]
        private async Task EditarSedeAsync()
        {
            if (SedeSeleccionada == null) return;
            if (_dialogService.ShowDialog<SedeEditorViewModel>(vm => vm.SetEntidad(SedeSeleccionada)) == true)
            {
                await BuscarSedesAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteSede))]
        private async Task EliminarSedeAsync()
        {
            if (SedeSeleccionada == null) return;
            if (ConfirmAction($"¿Eliminar '{SedeSeleccionada.Nombre}' y todas sus áreas y zonas?", "Confirmar"))
            {
                await _sedeSvc.EliminarAsync(SedeSeleccionada.Id);
                await BuscarSedesAsync();
            }
        }

        // --- AREAS ---
        private async Task BuscarAreasAsync()
        {
            Areas.Clear();
            if (SedeSeleccionada != null)
            {
                var areas = await _areaSvc.BuscarAsync(SedeSeleccionada.Id, null, default);
                foreach (var a in areas) Areas.Add(a);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteSede))]
        private async Task CrearAreaAsync()
        {
            if (SedeSeleccionada == null) return;
            var nueva = new Area { Activo = true, SedeId = SedeSeleccionada.Id };
            if (_dialogService.ShowDialog<AreaEditorViewModel>(vm => vm.SetEntidad(nueva, SedeSeleccionada)) == true)
            {
                await BuscarAreasAsync();
            }
        }

        private bool CanEditDeleteArea() => AreaSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditDeleteArea))]
        private async Task EditarAreaAsync()
        {
            if (AreaSeleccionada == null || SedeSeleccionada == null) return;
            if (_dialogService.ShowDialog<AreaEditorViewModel>(vm => vm.SetEntidad(AreaSeleccionada, SedeSeleccionada)) == true)
            {
                await BuscarAreasAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteArea))]
        private async Task EliminarAreaAsync()
        {
            if (AreaSeleccionada == null) return;
            if (ConfirmAction($"¿Eliminar '{AreaSeleccionada.Nombre}' y todas sus zonas?", "Confirmar"))
            {
                await _areaSvc.EliminarAsync(AreaSeleccionada.Id, default);
                await BuscarAreasAsync();
            }
        }

        // --- ZONAS ---
        private async Task BuscarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada != null)
            {
                var zonas = await _zonaSvc.BuscarAsync(AreaSeleccionada.Id, null, default);
                foreach (var z in zonas) Zonas.Add(z);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteArea))]
        private async Task CrearZonaAsync()
        {
            if (AreaSeleccionada == null) return;
            var nueva = new Zona { Activo = true, AreaId = AreaSeleccionada.Id };
            if (_dialogService.ShowDialog<ZonaEditorViewModel>(vm => vm.SetEntidad(nueva, AreaSeleccionada)) == true)
            {
                await BuscarZonasAsync();
            }
        }

        private bool CanEditDeleteZona() => ZonaSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditDeleteZona))]
        private async Task EditarZonaAsync()
        {
            if (ZonaSeleccionada == null || AreaSeleccionada == null) return;
            if (_dialogService.ShowDialog<ZonaEditorViewModel>(vm => vm.SetEntidad(ZonaSeleccionada, AreaSeleccionada)) == true)
            {
                await BuscarZonasAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditDeleteZona))]
        private async Task EliminarZonaAsync()
        {
            if (ZonaSeleccionada == null) return;
            if (ConfirmAction($"¿Eliminar la zona '{ZonaSeleccionada.Nombre}'?", "Confirmar"))
            {
                await _zonaSvc.EliminarAsync(ZonaSeleccionada.Id, default);
                await BuscarZonasAsync();
            }
        }
    }
}