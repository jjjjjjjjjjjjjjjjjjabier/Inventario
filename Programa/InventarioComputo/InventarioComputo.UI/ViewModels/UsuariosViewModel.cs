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
    public partial class UsuariosViewModel : BaseViewModel, IDisposable
    {
        private readonly IUsuarioService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        [NotifyCanExecuteChangedFor(nameof(AdministrarRolesCommand))]
        private UsuarioViewModel? _usuarioSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        private string _filtro = string.Empty;

        // Agregar esta propiedad para resolver el error de binding
        public string FiltroTexto
        {
            get => Filtro;
            set => Filtro = value;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        [NotifyCanExecuteChangedFor(nameof(AdministrarRolesCommand))]
        private bool _esAdministrador;

        public ObservableCollection<UsuarioViewModel> Usuarios { get; } = new();

        public UsuariosViewModel(
            IUsuarioService srv,
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<UsuariosViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = log;

            EsAdministrador = _sessionService.TieneRol("Administradores");
            _sessionService.SesionCambiada += OnSesionCambiada;
        }

        private void OnSesionCambiada(object? sender, bool estaLogueado)
        {
            EsAdministrador = _sessionService.TieneRol("Administradores");
            ActualizarCanExecute();
        }

        private void ActualizarCanExecute()
        {
            CrearCommand?.NotifyCanExecuteChanged();
            EditarCommand?.NotifyCanExecuteChanged();
            EliminarCommand?.NotifyCanExecuteChanged();
            AdministrarRolesCommand?.NotifyCanExecuteChanged();
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();
        partial void OnFiltroChanged(string value) => _ = BuscarAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Usuarios.Clear();
                var filtro = string.IsNullOrWhiteSpace(Filtro) ? null : Filtro.Trim();
                var listaUsuarios = await _srv.BuscarAsync(filtro, MostrarInactivos);
                
                foreach (var usuario in listaUsuarios)
                {
                    // Cargar roles para cada usuario
                    var roles = await _srv.ObtenerRolesDeUsuarioAsync(usuario.Id);
                    Usuarios.Add(new UsuarioViewModel(usuario, roles));
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando usuarios");
                _dialogService.ShowError("Ocurrió un error al cargar los usuarios.");
            }
            finally
            {
                IsBusy = false;
                ActualizarCanExecute();
            }
        }

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        private async Task CrearAsync()
        {
            var nuevo = new Usuario { Activo = true };
            if (_dialogService.ShowDialog<UsuarioEditorViewModel>(vm => vm.SetEntidad(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && UsuarioSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (UsuarioSeleccionado == null) return;

            var copia = new Usuario
            {
                Id = UsuarioSeleccionado.Id,
                NombreUsuario = UsuarioSeleccionado.NombreUsuario,
                NombreCompleto = UsuarioSeleccionado.NombreCompleto,
                Activo = UsuarioSeleccionado.Activo,
                EsEmpleadoSolamente = UsuarioSeleccionado.EsEmpleadoSolamente
            };

            if (_dialogService.ShowDialog<UsuarioEditorViewModel>(vm => vm.SetEntidad(copia)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (UsuarioSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Desactivar el usuario '{UsuarioSeleccionado.NombreUsuario}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(UsuarioSeleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al desactivar usuario");
                _dialogService.ShowError("No se pudo desactivar el usuario. Es posible que tenga equipos asociados.");
            }
            finally
            {
                IsBusy = false;
                ActualizarCanExecute();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task AdministrarRolesAsync()
        {
            if (UsuarioSeleccionado == null) return;

            // Abrir el diálogo para administrar roles
            bool? resultado = _dialogService.ShowDialog<UsuarioRolesViewModel>(
                vm => vm.CargarAsync(UsuarioSeleccionado.Id, UsuarioSeleccionado.NombreUsuario).Wait()
            );

            if (resultado == true)
            {
                // Recargar la lista de usuarios para reflejar los cambios
                await BuscarAsync();
            }
        }

        public void Dispose()
        {
            _sessionService.SesionCambiada -= OnSesionCambiada;
        }
    }
}