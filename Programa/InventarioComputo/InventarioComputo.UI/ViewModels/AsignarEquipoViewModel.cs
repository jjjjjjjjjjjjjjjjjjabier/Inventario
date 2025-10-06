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
    public partial class AsignarEquipoViewModel : BaseViewModel
    {
        private readonly IMovimientoService _movimientoService;
        private readonly IEmpleadoService _empleadoService;
        private readonly ISedeService _sedeService;
        private readonly IAreaService _areaService;
        private readonly IZonaService _zonaService;
        private readonly IDialogService _dialogService;

        private EquipoComputo _equipo = null!;

        [ObservableProperty] private string _titulo = "Asignar Equipo";
        [ObservableProperty] private string _numeroSerie = string.Empty;
        [ObservableProperty] private string _etiqueta = string.Empty;
        [ObservableProperty] private string _descripcionEquipo = string.Empty;
        [ObservableProperty] private string _asignacionActual = string.Empty;

        [ObservableProperty] private Empleado? _empleadoActual;
        [ObservableProperty] private Zona? _ubicacionActual;

        [ObservableProperty] private bool _asignarAEmpleado = true;
        [ObservableProperty] private bool _asignarAUbicacion;

        partial void OnAsignarAEmpleadoChanged(bool value) { if (value) AsignarAUbicacion = false; }
        partial void OnAsignarAUbicacionChanged(bool value) { if (value) AsignarAEmpleado = false; }

        [ObservableProperty] private Empleado? _empleadoSeleccionado;

        public ObservableCollection<Empleado> Empleados { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        [ObservableProperty] private Sede? _sedeSeleccionada;
        [ObservableProperty] private Area? _areaSeleccionada;
        [ObservableProperty] private Zona? _zonaSeleccionada;

        [ObservableProperty] private string _motivo = string.Empty;

        public bool DialogResult { get; set; }

        public AsignarEquipoViewModel(
            IMovimientoService movimientoService,
            IEmpleadoService empleadoService,
            ISedeService sedeService,
            IAreaService areaService,
            IZonaService zonaService,
            IDialogService dialogService,
            ILogger<AsignarEquipoViewModel> logger)
        {
            _movimientoService = movimientoService;
            _empleadoService = empleadoService;
            _sedeService = sedeService;
            _areaService = areaService;
            _zonaService = zonaService;
            _dialogService = dialogService;
            Logger = logger;
        }

        public async Task InitializeAsync(EquipoComputo equipo)
        {
            _equipo = equipo;
            NumeroSerie = equipo.NumeroSerie;
            Etiqueta = equipo.EtiquetaInventario;
            DescripcionEquipo = $"{equipo.Marca} {equipo.Modelo} - {equipo.TipoEquipo?.Nombre}";

            EmpleadoActual = equipo.Empleado;
            UbicacionActual = equipo.Zona;

            AsignacionActual = EmpleadoActual != null
                ? $"Empleado: {EmpleadoActual.NombreCompleto}"
                : UbicacionActual != null
                    ? $"Ubicación: {UbicacionActual.Area?.Sede?.Nombre} / {UbicacionActual.Area?.Nombre} / {UbicacionActual.Nombre}"
                    : "Sin asignación";

            EmpleadoSeleccionado = EmpleadoActual;

            await CargarCombosAsync();

            if (UbicacionActual != null)
            {
                try
                {
                    var zona = await _zonaService.ObtenerPorIdAsync(UbicacionActual.Id);
                    if (zona != null)
                    {
                        var area = await _areaService.ObtenerPorIdAsync(zona.AreaId);
                        if (area != null)
                        {
                            var sede = await _sedeService.ObtenerPorIdAsync(area.SedeId);
                            if (sede != null)
                            {
                                SedeSeleccionada = BuscarEn(Sedes, sede.Id);
                                await CargarAreasAsync();
                                AreaSeleccionada = BuscarEn(Areas, area.Id);
                                await CargarZonasAsync();
                                ZonaSeleccionada = BuscarEn(Zonas, zona.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogWarning(ex, "No se pudo preseleccionar la ubicación.");
                }
            }
        }

        private static T? BuscarEn<T>(ObservableCollection<T> source, int id) where T : class
        {
            foreach (var item in source)
            {
                if (item is Sede s && s.Id == id) return item;
                if (item is Area a && a.Id == id) return item;
                if (item is Zona z && z.Id == id) return item;
            }
            return null;
        }

        private async Task CargarCombosAsync()
        {
            try
            {
                IsBusy = true;

                Empleados.Clear();
                Sedes.Clear();
                Areas.Clear();
                Zonas.Clear();

                var empleados = await _empleadoService.ObtenerTodosAsync(false);
                foreach (var e in empleados) Empleados.Add(e);

                var sedes = await _sedeService.BuscarAsync(null, incluirInactivas: false);
                foreach (var s in sedes) Sedes.Add(s);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar datos para asignación");
                _dialogService.ShowError("No se pudieron cargar los datos necesarios para la asignación.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();
            ZonaSeleccionada = null;
            if (SedeSeleccionada == null) return;

            try
            {
                var areas = await _areaService.BuscarAsync(SedeSeleccionada.Id, null, incluirInactivas: true);
                foreach (var a in areas) Areas.Add(a);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar áreas (AsignarEquipo)");
            }
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            ZonaSeleccionada = null;
            if (AreaSeleccionada == null) return;

            try
            {
                var zonas = await _zonaService.BuscarAsync(AreaSeleccionada.Id, null, incluirInactivas: true);
                foreach (var z in zonas) Zonas.Add(z);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar zonas (AsignarEquipo)");
            }
        }

        partial void OnEmpleadoSeleccionadoChanged(Empleado? value)
        {
            if (value != null)
            {
                AsignarAEmpleado = true;
                AsignarAUbicacion = false;
            }
        }

        partial void OnSedeSeleccionadaChanged(Sede? value)
        {
            AsignarAUbicacion = true;
            _ = CargarAreasAsync();
        }

        partial void OnAreaSeleccionadaChanged(Area? value)
        {
            AsignarAUbicacion = true;
            _ = CargarZonasAsync();
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task AsignarAsync()
        {
            if (string.IsNullOrWhiteSpace(Motivo))
            {
                _dialogService.ShowError("Debe ingresar un motivo para el movimiento.");
                return;
            }

            if (!AsignarAEmpleado && !AsignarAUbicacion)
            {
                _dialogService.ShowError("Seleccione el tipo de asignación.");
                return;
            }

            if (AsignarAEmpleado && EmpleadoSeleccionado == null)
            {
                _dialogService.ShowError("Debe seleccionar un empleado.");
                return;
            }

            if (AsignarAUbicacion && (SedeSeleccionada == null || AreaSeleccionada == null || ZonaSeleccionada == null))
            {
                _dialogService.ShowError("Debe seleccionar sede, área y zona.");
                return;
            }

            try
            {
                IsBusy = true;
                await _movimientoService.AsignarEquipoAsync(
                    _equipo.Id,
                    AsignarAEmpleado ? EmpleadoSeleccionado?.Id : null,
                    AsignarAUbicacion ? ZonaSeleccionada?.Id : null,
                    Motivo.Trim());

                _dialogService.ShowInfo("Equipo asignado correctamente.");
                DialogResult = true;
                CerrarVentana();
            }
            catch (Exception ex)
            {
                // Muestra todo el árbol de excepciones
                var fullMessage = GetFullExceptionMessage(ex);
                Logger?.LogError(ex, "Error al asignar equipo: {Message}", fullMessage);
                _dialogService.ShowError($"Error al asignar: {fullMessage}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Cancelar() => CerrarVentana();

        private void CerrarVentana()
        {
            foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            {
                if (w.DataContext == this)
                {
                    w.Close();
                    break;
                }
            }
        }

        // Método auxiliar para obtener el mensaje completo de excepción
        private string GetFullExceptionMessage(Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            var currentEx = ex;
            int level = 0;

            while (currentEx != null)
            {
                sb.AppendLine($"[{level}] {currentEx.GetType().Name}: {currentEx.Message}");
                currentEx = currentEx.InnerException;
                level++;
            }

            return sb.ToString();
        }
    }
}