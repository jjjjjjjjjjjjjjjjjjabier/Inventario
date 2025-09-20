using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        private async Task CargarCombosAsync()
        {
            IsBusy = true;
            try
            {
                // Cargar catálogos
                var tipos = await _tipoEquipoSrv.BuscarAsync(null, false);
                foreach (var t in tipos) TiposEquipo.Add(t);

                var estados = await _estadoSrv.BuscarAsync(null, false);
                foreach (var e in estados) Estados.Add(e);

                var sedes = await _sedeSrv.BuscarAsync(null, false);
                foreach (var s in sedes) Sedes.Add(s);

                // Seleccionar valores si es una edición
                if (_entidad.Id > 0)
                {
                    TipoEquipoSeleccionado = TiposEquipo.FirstOrDefault(t => t.Id == _entidad.TipoEquipoId);
                    EstadoSeleccionado = Estados.FirstOrDefault(e => e.Id == _entidad.EstadoId);
                    if (_entidad.Zona != null)
                    {
                        SedeSeleccionada = Sedes.FirstOrDefault(s => s.Id == _entidad.Zona.Area.SedeId);
                        // Cargar áreas y zonas en cascada...
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando catálogos para el editor de equipos");
                _dialogService.ShowError("No se pudieron cargar los datos necesarios para el formulario.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSedeSeleccionadaChanged(Sede? value) => _ = CargarAreasAsync();
        partial void OnAreaSeleccionadaChanged(Area? value) => _ = CargarZonasAsync();

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();
            if (SedeSeleccionada != null)
            {
                var areas = await _areaSrv.BuscarAsync(SedeSeleccionada.Id, null, default);
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
                var zonas = await _zonaSrv.BuscarAsync(AreaSeleccionada.Id, null, default);
                foreach (var z in zonas) Zonas.Add(z);

                if (_entidad.ZonaId > 0)
                    ZonaSeleccionada = Zonas.FirstOrDefault(z => z.Id == _entidad.ZonaId);
            }
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            try
            {
                // Asignar Ids de los combos a la entidad
                _entidad.TipoEquipoId = TipoEquipoSeleccionado?.Id ?? 0;
                _entidad.EstadoId = EstadoSeleccionado?.Id ?? 0;
                _entidad.ZonaId = ZonaSeleccionada?.Id ?? 0;

                if (_entidad.Id == 0)
                    await _equipoSrv.AgregarAsync(_entidad);
                else
                    await _equipoSrv.ActualizarAsync(_entidad);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar equipo");
                _dialogService.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }
    }
}