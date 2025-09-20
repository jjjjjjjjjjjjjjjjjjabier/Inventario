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

        [ObservableProperty]
        private EquipoComputo? _entidad;

        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();
        public ObservableCollection<Estado> Estados { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        [ObservableProperty]
        private TipoEquipo? _tipoEquipoSeleccionado;

        [ObservableProperty]
        private Estado? _estadoSeleccionado;

        [ObservableProperty]
        private Sede? _sedeSeleccionada;

        [ObservableProperty]
        private Area? _areaSeleccionada;

        [ObservableProperty]
        private Zona? _zonaSeleccionada;

        public EquipoComputoEditorViewModel(IEquipoComputoService equipoSrv, ITipoEquipoService tipoEquipoSrv, IEstadoService estadoSrv, ISedeService sedeSrv, IAreaService areaSrv, IZonaService zonaSrv, ILogger<EquipoComputoEditorViewModel> log)
        {
            _equipoSrv = equipoSrv;
            _tipoEquipoSrv = tipoEquipoSrv;
            _estadoSrv = estadoSrv;
            _sedeSrv = sedeSrv;
            _areaSrv = areaSrv;
            _zonaSrv = zonaSrv;
            Logger = log;
        }

        public async void SetEquipo(EquipoComputo equipo)
        {
            Entidad = equipo;
            await CargarComboBoxes();

            TipoEquipoSeleccionado = TiposEquipo.FirstOrDefault(t => t.Id == equipo.TipoEquipoId);
            EstadoSeleccionado = Estados.FirstOrDefault(e => e.Id == equipo.EstadoId);

            if (equipo.Zona != null)
            {
                SedeSeleccionada = Sedes.FirstOrDefault(s => s.Id == equipo.Zona.Area?.SedeId);
                await CargarAreasAsync();
                AreaSeleccionada = Areas.FirstOrDefault(a => a.Id == equipo.Zona.AreaId);
                await CargarZonasAsync();
                ZonaSeleccionada = Zonas.FirstOrDefault(z => z.Id == equipo.ZonaId);
            }
        }

        partial void OnSedeSeleccionadaChanged(Sede? value) => _ = CargarAreasAsync();
        partial void OnAreaSeleccionadaChanged(Area? value) => _ = CargarZonasAsync();

        private async Task CargarComboBoxes()
        {
            IsBusy = true;
            try
            {
                TiposEquipo.Clear();
                var tipos = await _tipoEquipoSrv.BuscarAsync(null, true);
                foreach (var t in tipos) TiposEquipo.Add(t);

                Estados.Clear();
                var estados = await _estadoSrv.BuscarAsync(null, true);
                foreach (var e in estados) Estados.Add(e);

                Sedes.Clear();
                var sedes = await _sedeSrv.BuscarAsync(null, true);
                foreach (var s in sedes) Sedes.Add(s);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando ComboBoxes para Equipo Editor");
                ShowError("Error al cargar datos necesarios para el formulario.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            if (SedeSeleccionada != null)
            {
                var areas = await _areaSrv.BuscarAsync(SedeSeleccionada.Id, null, default);
                foreach (var a in areas) Areas.Add(a);
            }
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada != null)
            {
                var zonas = await _zonaSrv.BuscarAsync(AreaSeleccionada.Id, null, default);
                foreach (var z in zonas) Zonas.Add(z);
            }
        }

        [RelayCommand]
        private async Task GuardarAsync(Window window)
        {
            if (Entidad == null || string.IsNullOrWhiteSpace(Entidad.NumeroSerie) || TipoEquipoSeleccionado == null || EstadoSeleccionado == null || ZonaSeleccionada == null)
            {
                ShowError("Número de Serie, Tipo, Estado y Zona son obligatorios.");
                return;
            }

            IsBusy = true;
            try
            {
                Entidad.TipoEquipoId = TipoEquipoSeleccionado.Id;
                Entidad.EstadoId = EstadoSeleccionado.Id;
                Entidad.ZonaId = ZonaSeleccionada.Id;

                if (Entidad.Id == 0) await _equipoSrv.AgregarAsync(Entidad);
                else await _equipoSrv.ActualizarAsync(Entidad);

                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error guardando equipo");
                ShowError($"No se pudo guardar el equipo. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}