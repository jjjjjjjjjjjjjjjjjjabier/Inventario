using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace InventarioComputo.UI.ViewModels
{
    public partial class AsignarEquipoViewModel : BaseViewModel
    {
        private readonly IMovimientoService _movimientoSvc;
        private readonly IUsuarioService _usuarioSvc;
        private readonly IZonaService _zonaSvc;
        private readonly IAreaService _areaSvc;
        private readonly ISedeService _sedeSvc;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        private int _equipoId;
        private EquipoComputo _equipo;

        [ObservableProperty]
        private string _titulo;

        [ObservableProperty]
        private string _motivoMovimiento = string.Empty;

        [ObservableProperty]
        private Usuario _usuarioSeleccionado;

        [ObservableProperty]
        private Sede _sedeSeleccionada;

        [ObservableProperty]
        private Area _areaSeleccionada;

        [ObservableProperty]
        private Zona _zonaSeleccionada;

        public event EventHandler RequestClose;
        public bool DialogResult { get; set; }

        public ObservableCollection<Usuario> Usuarios { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        public AsignarEquipoViewModel(
            IMovimientoService movimientoSvc,
            IUsuarioService usuarioSvc,
            IZonaService zonaSvc,
            IAreaService areaSvc,
            ISedeService sedeSvc,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<AsignarEquipoViewModel> logger)
        {
            _movimientoSvc = movimientoSvc;
            _usuarioSvc = usuarioSvc;
            _zonaSvc = zonaSvc;
            _areaSvc = areaSvc;
            _sedeSvc = sedeSvc;
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = logger;
            Titulo = "Asignar Equipo";
        }

        partial void OnSedeSeleccionadaChanged(Sede value)
        {
            Areas.Clear();
            Zonas.Clear();
            AreaSeleccionada = null;
            ZonaSeleccionada = null;
            _ = CargarAreasAsync();
        }

        partial void OnAreaSeleccionadaChanged(Area value)
        {
            Zonas.Clear();
            ZonaSeleccionada = null;
            _ = CargarZonasAsync();
        }

        public async Task InitializeAsync(EquipoComputo equipo)
        {
            _equipo = equipo;
            _equipoId = equipo.Id;
            Titulo = $"Asignar Equipo: {equipo.NumeroSerie}";

            await CargarUsuariosAsync();
            await CargarSedesAsync();

            if (equipo.UsuarioId.HasValue)
            {
                UsuarioSeleccionado = Usuarios.FirstOrDefault(u => u.Id == equipo.UsuarioId);
            }

            if (equipo.SedeId.HasValue)
            {
                SedeSeleccionada = Sedes.FirstOrDefault(s => s.Id == equipo.SedeId);
                if (SedeSeleccionada != null && equipo.AreaId.HasValue)
                {
                    await CargarAreasAsync();
                    AreaSeleccionada = Areas.FirstOrDefault(a => a.Id == equipo.AreaId);
                    if (AreaSeleccionada != null && equipo.ZonaId.HasValue)
                    {
                        await CargarZonasAsync();
                        ZonaSeleccionada = Zonas.FirstOrDefault(z => z.Id == equipo.ZonaId);
                    }
                }
            }
        }

        private async Task CargarUsuariosAsync()
        {
            try
            {
                Usuarios.Clear();
                var usuarios = await _usuarioSvc.BuscarAsync(null, false);
                foreach (var usuario in usuarios)
                {
                    Usuarios.Add(usuario);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar usuarios");
            }
        }

        private async Task CargarSedesAsync()
        {
            try
            {
                Sedes.Clear();
                var sedes = await _sedeSvc.BuscarAsync(null, false);
                foreach (var sede in sedes)
                {
                    Sedes.Add(sede);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar sedes");
            }
        }

        private async Task CargarAreasAsync()
        {
            if (SedeSeleccionada == null) return;

            try
            {
                Areas.Clear();
                var areas = await _areaSvc.BuscarAsync(SedeSeleccionada.Id, null, false);
                foreach (var area in areas)
                {
                    Areas.Add(area);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar áreas");
            }
        }

        private async Task CargarZonasAsync()
        {
            if (AreaSeleccionada == null) return;

            try
            {
                Zonas.Clear();
                var zonas = await _zonaSvc.BuscarAsync(AreaSeleccionada.Id, null, false);
                foreach (var zona in zonas)
                {
                    Zonas.Add(zona);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar zonas");
            }
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(MotivoMovimiento))
            {
                _dialogService.ShowError("Debe ingresar un motivo para el movimiento.");
                return;
            }

            IsBusy = true;
            try
            {
                int? usuarioId = UsuarioSeleccionado?.Id;
                int? zonaId = ZonaSeleccionada?.Id;

                if (_sessionService.UsuarioActual != null)
                {
                    await _movimientoSvc.RegistrarMovimientoAsync(
                        _equipoId,
                        usuarioId,
                        zonaId,
                        MotivoMovimiento,
                        _sessionService.UsuarioActual.Id);

                    DialogResult = true;
                    _dialogService.ShowInfo("Equipo asignado correctamente.");
                    RequestClose?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _dialogService.ShowError("No hay una sesión de usuario activa.");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al asignar equipo");
                _dialogService.ShowError($"Error al asignar equipo: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            DialogResult = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}