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
    public partial class AsignarEquipoViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IEquipoComputoService _equipoSrv;
        private readonly IMovimientoService _movimientoSrv;
        private readonly IUsuarioService _usuarioSrv;
        private readonly ISessionService _sessionService;
        private readonly ISedeService _sedeSrv;
        private readonly IAreaService _areaSrv;
        private readonly IZonaService _zonaSrv;
        private readonly IDialogService _dialogService;

        private EquipoComputo _equipo = new();

        [ObservableProperty]
        private string _titulo = "Asignar Equipo";

        [ObservableProperty]
        private Usuario? _usuarioActual;

        [ObservableProperty]
        private Zona? _zonaActual;

        [ObservableProperty]
        private string _motivo = string.Empty;

        // Propiedades para selecciones
        [ObservableProperty]
        private Usuario? _usuarioSeleccionado;

        [ObservableProperty]
        private Sede? _sedeSeleccionada;

        [ObservableProperty]
        private Area? _areaSeleccionada;

        [ObservableProperty]
        private Zona? _zonaSeleccionada;

        // Colecciones para los combos
        public ObservableCollection<Usuario> Usuarios { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        public bool DialogResult { get; set; }

        public AsignarEquipoViewModel(
            IEquipoComputoService equipoSrv,
            IMovimientoService movimientoSrv,
            IUsuarioService usuarioSrv,
            ISessionService sessionService,
            ISedeService sedeSrv,
            IAreaService areaSrv,
            IZonaService zonaSrv,
            IDialogService dialogService,
            ILogger<AsignarEquipoViewModel> logger)
        {
            _equipoSrv = equipoSrv;
            _movimientoSrv = movimientoSrv;
            _usuarioSrv = usuarioSrv;
            _sessionService = sessionService;
            _sedeSrv = sedeSrv;
            _areaSrv = areaSrv;
            _zonaSrv = zonaSrv;
            _dialogService = dialogService;
            Logger = logger;
        }

        public void SetEquipo(EquipoComputo equipo)
        {
            _equipo = equipo;
            Titulo = $"Asignar/Mover Equipo: {equipo.NumeroSerie}";
            
            // Cargar datos actuales
            _ = CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            IsBusy = true;
            try
            {
                // 1. Cargar usuario y zona actuales
                if (_equipo.UsuarioId.HasValue)
                {
                    UsuarioActual = await _usuarioSrv.ObtenerPorIdAsync(_equipo.UsuarioId.Value);
                }

                if (_equipo.ZonaId.HasValue)
                {
                    ZonaActual = await _zonaSrv.ObtenerPorIdAsync(_equipo.ZonaId.Value);
                }

                // 2. Cargar listas para selección
                var usuarios = await _usuarioSrv.BuscarAsync(null, false);
                Usuarios.Clear();
                foreach (var u in usuarios) Usuarios.Add(u);

                var sedes = await _sedeSrv.BuscarAsync(null, false);
                Sedes.Clear();
                foreach (var s in sedes) Sedes.Add(s);

                // 3. Preseleccionar sede y área actuales si existen
                if (ZonaActual != null)
                {
                    var areaActual = await _areaSrv.ObtenerPorIdAsync(ZonaActual.AreaId);
                    if (areaActual != null)
                    {
                        SedeSeleccionada = Sedes.FirstOrDefault(s => s.Id == areaActual.SedeId);
                        
                        // Esto disparará la carga de áreas, y luego:
                        await CargarAreasAsync();
                        AreaSeleccionada = Areas.FirstOrDefault(a => a.Id == areaActual.Id);
                        
                        // Esto disparará la carga de zonas:
                        await CargarZonasAsync();
                        ZonaSeleccionada = Zonas.FirstOrDefault(z => z.Id == ZonaActual.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar datos para asignar equipo");
                _dialogService.ShowError("Error al cargar los datos necesarios.");
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
            ZonaSeleccionada = null;
            AreaSeleccionada = null;

            if (SedeSeleccionada != null)
            {
                try
                {
                    var areas = await _areaSrv.BuscarAsync(SedeSeleccionada.Id, null, default);
                    foreach (var a in areas) Areas.Add(a);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error al cargar áreas para sede {SedeId}", SedeSeleccionada.Id);
                }
            }
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            ZonaSeleccionada = null;

            if (AreaSeleccionada != null)
            {
                try
                {
                    var zonas = await _zonaSrv.BuscarAsync(AreaSeleccionada.Id, null, default);
                    foreach (var z in zonas) Zonas.Add(z);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error al cargar zonas para área {AreaId}", AreaSeleccionada.Id);
                }
            }
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(Motivo))
            {
                _dialogService.ShowError("Debe proporcionar un motivo para el movimiento.");
                return;
            }

            if (UsuarioSeleccionado == null && ZonaSeleccionada == null)
            {
                _dialogService.ShowError("Debe seleccionar un nuevo usuario o ubicación.");
                return;
            }

            if (Motivo.Length > 500)
            {
                _dialogService.ShowError("El motivo no puede exceder 500 caracteres.");
                return;
            }

            // Verificar si hay cambio efectivo
            bool hayNuevoUsuario = UsuarioSeleccionado != null && 
                                  (UsuarioActual == null || UsuarioSeleccionado.Id != UsuarioActual.Id);
            
            bool hayNuevaZona = ZonaSeleccionada != null && 
                               (ZonaActual == null || ZonaSeleccionada.Id != ZonaActual.Id);

            if (!hayNuevoUsuario && !hayNuevaZona)
            {
                _dialogService.ShowError("No se detectaron cambios en la asignación.");
                return;
            }

            IsBusy = true;
            try
            {
                // Registrar el movimiento (esto actualiza el equipo y crea la entrada en el historial)
                await _movimientoSrv.RegistrarMovimientoAsync(
                    _equipo.Id,
                    UsuarioSeleccionado?.Id,
                    ZonaSeleccionada?.Id,
                    Motivo,
                    _sessionService.UsuarioActual!.Id);

                _dialogService.ShowInfo("Movimiento registrado correctamente.");
                DialogResult = true;
                System.Windows.Application.Current.Windows.OfType<Window>()
                    .SingleOrDefault(w => w.DataContext == this)?.Close();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al registrar movimiento para equipo {EquipoId}", _equipo.Id);
                _dialogService.ShowError($"Error al registrar el movimiento: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Close()
        {
            System.Windows.Application.Current.Windows.OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this)?.Close();
        }
    }
}