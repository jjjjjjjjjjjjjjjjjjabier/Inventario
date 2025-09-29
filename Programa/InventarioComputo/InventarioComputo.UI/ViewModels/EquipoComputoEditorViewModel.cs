using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Extensions;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InventarioComputo.UI.ViewModels
{
    public partial class EquipoComputoEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IEquipoComputoService _equipoSrv;
        private readonly ITipoEquipoService _tipoEquipoSrv;
        private readonly IEstadoService _estadoSrv;
        private readonly ISedeService _sedeSrv;
        private readonly IAreaService _areaSrv;
        private readonly IZonaService _zonaSrv;
        private readonly IDialogService _dialogService;

        private EquipoComputo _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nuevo Equipo";
        public bool DialogResult { get; set; }

        // --- Propiedades del formulario ---
        public string NumeroSerie { get => _entidad.NumeroSerie; set => SetProperty(_entidad.NumeroSerie, value, _entidad, (e, v) => e.NumeroSerie = v); }
        public string EtiquetaInventario { get => _entidad.EtiquetaInventario; set => SetProperty(_entidad.EtiquetaInventario, value, _entidad, (e, v) => e.EtiquetaInventario = v); }
        public string Marca { get => _entidad.Marca; set => SetProperty(_entidad.Marca, value, _entidad, (e, v) => e.Marca = v); }
        public string Modelo { get => _entidad.Modelo; set => SetProperty(_entidad.Modelo, value, _entidad, (e, v) => e.Modelo = v); }
        public string Caracteristicas { get => _entidad.Caracteristicas; set => SetProperty(_entidad.Caracteristicas, value, _entidad, (e, v) => e.Caracteristicas = v); }
        public string? Observaciones { get => _entidad.Observaciones; set => SetProperty(_entidad.Observaciones, value, _entidad, (e, v) => e.Observaciones = v); }
        public DateTime? FechaAdquisicion { get => _entidad.FechaAdquisicion; set => SetProperty(_entidad.FechaAdquisicion, value, _entidad, (e, v) => e.FechaAdquisicion = v); }

        // --- Propiedades para ComboBoxes ---
        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();
        public ObservableCollection<Estado> Estados { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        [ObservableProperty] private TipoEquipo? _tipoEquipoSeleccionado;
        [ObservableProperty] private Estado? _estadoSeleccionado;
        [ObservableProperty] private Sede? _sedeSeleccionada;
        [ObservableProperty] private Area? _areaSeleccionada;
        [ObservableProperty] private Zona? _zonaSeleccionada;

        public EquipoComputoEditorViewModel(
            IEquipoComputoService equipoSrv, ITipoEquipoService tipoEquipoSrv, IEstadoService estadoSrv,
            ISedeService sedeSrv, IAreaService areaSrv, IZonaService zonaSrv,
            IDialogService dialogService, ILogger<EquipoComputoEditorViewModel> log)
        {
            _equipoSrv = equipoSrv;
            _tipoEquipoSrv = tipoEquipoSrv;
            _estadoSrv = estadoSrv;
            _sedeSrv = sedeSrv;
            _areaSrv = areaSrv;
            _zonaSrv = zonaSrv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEquipo(EquipoComputo equipo)
        {
            _entidad = equipo;
            if (equipo.Id > 0) Titulo = "Editar Equipo";
            else FechaAdquisicion = DateTime.Today;

            // Cargar los combos y luego seleccionar los valores del equipo
            _ = CargarCombosAsync();
        }

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();
            if (SedeSeleccionada != null)
            {
                // Incluir inactivas para poder preseleccionar áreas existentes
                var areas = await _areaSrv.BuscarAsync(SedeSeleccionada.Id, null, true, default);
                foreach (var a in areas) Areas.Add(a);

                if (_entidad.Zona?.AreaId > 0)
                    AreaSeleccionada = Areas.FirstOrDefault(a => a.Id == _entidad.Zona.AreaId);
            }
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada != null)
            {
                // Incluir inactivas para poder preseleccionar zonas existentes
                var zonas = await _zonaSrv.BuscarAsync(AreaSeleccionada.Id, null, true, default);
                foreach (var z in zonas) Zonas.Add(z);

                if (_entidad.ZonaId > 0)
                    ZonaSeleccionada = Zonas.FirstOrDefault(z => z.Id == _entidad.ZonaId);
            }
        }

        private async Task CargarCombosAsync()
        {
            IsBusy = true;
            try
            {
                // Cargar catálogos (incluye inactivas para preselección en ediciones)
                var tipos = await _tipoEquipoSrv.BuscarAsync(null, true);
                TiposEquipo.Clear();
                foreach (var t in tipos) TiposEquipo.Add(t);

                var estados = await _estadoSrv.BuscarAsync(null, true);
                Estados.Clear();
                foreach (var e in estados) Estados.Add(e);

                var sedes = await _sedeSrv.BuscarAsync(null, true);
                Sedes.Clear();
                foreach (var s in sedes) Sedes.Add(s);

                if (_entidad.Id > 0)
                {
                    TipoEquipoSeleccionado = TiposEquipo.FirstOrDefault(t => t.Id == _entidad.TipoEquipoId);
                    EstadoSeleccionado = Estados.FirstOrDefault(e => e.Id == _entidad.EstadoId);

                    if (_entidad.ZonaId.HasValue)
                    {
                        var zona = await _zonaSrv.ObtenerPorIdAsync(_entidad.ZonaId.Value);
                        if (zona != null)
                        {
                            var area = await _areaSrv.ObtenerPorIdAsync(zona.AreaId);
                            if (area != null)
                            {
                                SedeSeleccionada = Sedes.FirstOrDefault(s => s.Id == area.SedeId);
                                await CargarAreasAsync();
                                AreaSeleccionada = Areas.FirstOrDefault(a => a.Id == area.Id);
                                await CargarZonasAsync();
                                ZonaSeleccionada = Zonas.FirstOrDefault(z => z.Id == zona.Id);
                            }
                        }
                    }
                }
                else
                {
                    // valores por defecto opcionales para nuevo
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando catálogos para el editor de equipos");
                _dialogService.ShowError("No se pudieron cargar los datos necesarios para el formulario.");
            }
            finally { IsBusy = false; }
        }

        partial void OnSedeSeleccionadaChanged(Sede? value) => _ = CargarAreasAsync();
        partial void OnAreaSeleccionadaChanged(Area? value) => _ = CargarZonasAsync();

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NumeroSerie) || NumeroSerie.Length > 100)
            {
                _dialogService.ShowError("El número de serie es obligatorio y debe tener menos de 100 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(EtiquetaInventario) || EtiquetaInventario.Length > 100)
            {
                _dialogService.ShowError("La etiqueta es obligatoria y debe tener menos de 100 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Marca) || Marca.Length > 100)
            {
                _dialogService.ShowError("La marca es obligatoria y debe tener menos de 100 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Modelo) || Modelo.Length > 100)
            {
                _dialogService.ShowError("El modelo es obligatorio y debe tener menos de 100 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Caracteristicas) || Caracteristicas.Length > 500)
            {
                _dialogService.ShowError("Las características son obligatorias y deben tener menos de 500 caracteres.");
                return;
            }
            if (Observaciones?.Length > 1000)
            {
                _dialogService.ShowError("Las observaciones no pueden exceder 1000 caracteres.");
                return;
            }
            if (TipoEquipoSeleccionado == null)
            {
                _dialogService.ShowError("Debe seleccionar un tipo de equipo.");
                return;
            }
            if (EstadoSeleccionado == null)
            {
                _dialogService.ShowError("Debe seleccionar un estado.");
                return;
            }
            // Ubicación opcional: no forzamos ZonaSeleccionada

            try
            {
                _entidad.TipoEquipoId = TipoEquipoSeleccionado.Id;
                _entidad.EstadoId = EstadoSeleccionado.Id;
                _entidad.ZonaId = ZonaSeleccionada?.Id; // opcional

                if (_entidad.Id == 0)
                    await _equipoSrv.AgregarAsync(_entidad);
                else
                    await _equipoSrv.ActualizarAsync(_entidad);

                _dialogService.ShowInfo("Equipo guardado correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar equipo");
                _dialogService.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }

        [RelayCommand]
        public void Close()
        {
            this.CloseWindowOfViewModel();
        }
    }
}