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
        private readonly IZonaService _zonaService;
        private readonly IDialogService _dialogService;

        private EquipoComputo _equipo = null!;

        [ObservableProperty] private string _titulo = "Asignar Equipo";
        [ObservableProperty] private string _numeroSerie = string.Empty;
        [ObservableProperty] private string _etiqueta = string.Empty;
        [ObservableProperty] private string _descripcionEquipo = string.Empty;

        [ObservableProperty] private Empleado? _empleadoActual;
        [ObservableProperty] private Zona? _ubicacionActual;

        [ObservableProperty] private Empleado? _empleadoSeleccionado;
        [ObservableProperty] private Zona? _zonaSeleccionada;

        [ObservableProperty] private string _motivo = string.Empty;

        public ObservableCollection<Empleado> Empleados { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        public bool DialogResult { get; set; }

        public AsignarEquipoViewModel(
            IMovimientoService movimientoService,
            IEmpleadoService empleadoService,
            IZonaService zonaService,
            IDialogService dialogService,
            ILogger<AsignarEquipoViewModel> logger)
        {
            _movimientoService = movimientoService;
            _empleadoService = empleadoService;
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

            EmpleadoSeleccionado = EmpleadoActual;
            ZonaSeleccionada = UbicacionActual;

            await CargarCombosAsync();
        }

        private async Task CargarCombosAsync()
        {
            try
            {
                IsBusy = true;

                Empleados.Clear();
                Zonas.Clear();

                var empleados = await _empleadoService.ObtenerTodosAsync(false);
                foreach (var e in empleados) Empleados.Add(e);

                // OJO: el nombre del parámetro es incluirInactivas (no incluirInactivos)
                var zonas = await _zonaService.ObtenerTodasAsync(incluirInactivas: false);
                foreach (var z in zonas) Zonas.Add(z);
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

        [RelayCommand]
        public async Task AsignarAsync()
        {
            if (string.IsNullOrWhiteSpace(Motivo))
            {
                _dialogService.ShowError("Debe ingresar un motivo para el movimiento.");
                return;
            }
            if (EmpleadoSeleccionado == null && ZonaSeleccionada == null)
            {
                _dialogService.ShowError("Debe seleccionar un empleado o una ubicación.");
                return;
            }

            try
            {
                IsBusy = true;
                await _movimientoService.AsignarEquipoAsync(
                    _equipo.Id,
                    EmpleadoSeleccionado?.Id,
                    ZonaSeleccionada?.Id,
                    Motivo.Trim());
                _dialogService.ShowInfo("Equipo asignado correctamente.");
                DialogResult = true;
                CloseWindow();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al asignar equipo");
                _dialogService.ShowError("Ocurrió un error al realizar la asignación: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Cancelar() => CloseWindow();

        private void CloseWindow()
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
    }
}