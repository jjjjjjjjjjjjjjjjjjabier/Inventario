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
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class MovimientoEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IMovimientoService _movimientoSvc;
        private readonly IUsuarioService _usuarioSvc;
        private readonly IZonaService _zonaSvc;
        private readonly ISessionService _sessionService;
        private readonly IDialogService _dialogService;

        private int _equipoId;
        private string _equipoDescripcion = string.Empty;

        [ObservableProperty]
        private string _titulo = "Registrar Movimiento";

        [ObservableProperty]
        private string _motivo = string.Empty;

        [ObservableProperty]
        private Usuario? _usuarioNuevoSeleccionado;

        [ObservableProperty]
        private Zona? _zonaNuevaSeleccionada;

        public ObservableCollection<Usuario> Usuarios { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();

        public bool DialogResult { get; set; }

        public MovimientoEditorViewModel(
            IMovimientoService movimientoSvc,
            IUsuarioService usuarioSvc,
            IZonaService zonaSvc,
            ISessionService sessionService,
            IDialogService dialogService,
            ILogger<MovimientoEditorViewModel> log)
        {
            _movimientoSvc = movimientoSvc;
            _usuarioSvc = usuarioSvc;
            _zonaSvc = zonaSvc;
            _sessionService = sessionService;
            _dialogService = dialogService;
            Logger = log;
        }

        public void ConfigurarMovimiento(int equipoId, string equipoDescripcion, int? usuarioActualId, int? zonaActualId)
        {
            _equipoId = equipoId;
            _equipoDescripcion = equipoDescripcion;
            Titulo = $"Mover equipo: {equipoDescripcion}";

            _ = CargarUsuariosAsync();
            _ = CargarZonasAsync();
        }

        private async Task CargarUsuariosAsync()
        {
            try
            {
                Usuarios.Clear();
                var usuarios = await _usuarioSvc.BuscarAsync(null, true, System.Threading.CancellationToken.None);
                foreach (var u in usuarios) Usuarios.Add(u);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando usuarios");
            }
        }

        private async Task CargarZonasAsync()
        {
            try
            {
                Zonas.Clear();
                var zonas = await _zonaSvc.ObtenerTodasAsync(true);
                foreach (var z in zonas) Zonas.Add(z);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando zonas");
            }
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Motivo))
            {
                _dialogService.ShowError("El motivo del movimiento es obligatorio.");
                return;
            }

            if (UsuarioNuevoSeleccionado == null && ZonaNuevaSeleccionada == null)
            {
                _dialogService.ShowError("Debe seleccionar un nuevo usuario o una nueva ubicación.");
                return;
            }

            try
            {
                var usuarioResponsableId = _sessionService.UsuarioActual?.Id
                    ?? throw new InvalidOperationException("No hay usuario autenticado.");

                await _movimientoSvc.RegistrarMovimientoAsync(
                    _equipoId,
                    UsuarioNuevoSeleccionado?.Id,
                    ZonaNuevaSeleccionada?.Id,
                    Motivo,
                    usuarioResponsableId);

                _dialogService.ShowInfo("Movimiento registrado correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al registrar movimiento");
                _dialogService.ShowError($"Error al registrar el movimiento: {ex.Message}");
            }
        }

        [RelayCommand]
        public void Cancelar() => this.CloseWindowOfViewModel();
    }
}