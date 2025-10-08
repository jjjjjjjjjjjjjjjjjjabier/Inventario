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
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class UsuarioEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IUsuarioService _srv;
        private readonly IRolService _rolSrv;
        private readonly IDialogService _dialogService;

        private Usuario _entidad = new();

        [ObservableProperty] private string _titulo = "Nuevo Usuario";
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _confirmPassword = string.Empty;

        // NUEVO: estado de edición
        [ObservableProperty] private bool _esEdicion;
        partial void OnEsEdicionChanged(bool value) => AdministrarRolesCommand?.NotifyCanExecuteChanged();

        // Checkbox de solo empleado
        [ObservableProperty] private bool _esEmpleadoSolamente;

        public ObservableCollection<RolCheckboxViewModel> Roles { get; } = new();

        public string NombreUsuario
        {
            get => _entidad.NombreUsuario;
            set => SetProperty(_entidad.NombreUsuario, value, _entidad, (e, v) => e.NombreUsuario = v);
        }
        public string NombreCompleto
        {
            get => _entidad.NombreCompleto;
            set => SetProperty(_entidad.NombreCompleto, value, _entidad, (e, v) => e.NombreCompleto = v);
        }
        public bool Activo
        {
            get => _entidad.Activo;
            set => SetProperty(_entidad.Activo, value, _entidad, (e, v) => e.Activo = v);
        }
        public bool DialogResult { get; set; }

        public UsuarioEditorViewModel(IUsuarioService srv, IRolService rolSrv, IDialogService dialogService, ILogger<UsuarioEditorViewModel> log)
        {
            _srv = srv;
            _rolSrv = rolSrv;
            _dialogService = dialogService;
            Logger = log;
        }

        public async Task SetEntidad(Usuario usuario)
        {
            _entidad = usuario;
            Titulo = usuario.Id > 0 ? "Editar Usuario" : "Nuevo Usuario";
            EsEdicion = usuario.Id > 0;

            NombreUsuario = usuario.NombreUsuario;
            NombreCompleto = usuario.NombreCompleto;
            Activo = usuario.Activo;
            EsEmpleadoSolamente = usuario.EsEmpleadoSolamente;

            // Cargar roles solo en edición
            if (EsEdicion)
                await CargarRolesAsync();
        }

        private async Task CargarRolesAsync(CancellationToken ct = default)
        {
            try
            {
                Roles.Clear();
                var rolesDisponibles = await _rolSrv.ObtenerTodosAsync(true, ct);
                var rolesUsuario = _entidad.Id > 0 ? await _srv.ObtenerRolesDeUsuarioAsync(_entidad.Id, ct) : Array.Empty<Rol>();
                foreach (var rol in rolesDisponibles)
                    Roles.Add(new RolCheckboxViewModel(rol.Id, rol.Nombre, rolesUsuario.Any(r => r.Id == rol.Id)));
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando roles");
                _dialogService.ShowError("No se pudieron cargar los roles.");
            }
        }

        private static bool ValidarContraseña(string password) => !string.IsNullOrEmpty(password) && password.Length >= 6;

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario) || NombreUsuario.Length > 50)
            {
                _dialogService.ShowError("El nombre de usuario es obligatorio y debe tener menos de 50 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(NombreCompleto) || NombreCompleto.Length > 200)
            {
                _dialogService.ShowError("El nombre completo es obligatorio y debe tener menos de 200 caracteres.");
                return;
            }

            var crearNuevo = _entidad.Id == 0;
            if (crearNuevo)
            {
                if (string.IsNullOrWhiteSpace(Password))
                {
                    _dialogService.ShowError("La contraseña es obligatoria para nuevos usuarios.");
                    return;
                }
                if (!ValidarContraseña(Password))
                {
                    _dialogService.ShowError("La contraseña debe tener al menos 6 caracteres.");
                    return;
                }
                if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
                {
                    _dialogService.ShowError("La confirmación de contraseña no coincide.");
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Password) || !string.IsNullOrEmpty(ConfirmPassword))
                {
                    if (!ValidarContraseña(Password))
                    {
                        _dialogService.ShowError("La contraseña debe tener al menos 6 caracteres.");
                        return;
                    }
                    if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
                    {
                        _dialogService.ShowError("La confirmación de contraseña no coincide.");
                        return;
                    }
                }
            }

            try
            {
                var usuarioGuardado = await _srv.GuardarAsync(_entidad, string.IsNullOrEmpty(Password) ? null : Password);

                // Sincronizar roles (solo edición)
                if (usuarioGuardado.Id > 0 && Roles.Count > 0)
                {
                    var rolesSeleccionadosIds = Roles.Where(r => r.IsSelected).Select(r => r.RolId).ToHashSet();
                    var rolesActuales = await _srv.ObtenerRolesDeUsuarioAsync(usuarioGuardado.Id);
                    var actualesIds = rolesActuales.Select(r => r.Id).ToHashSet();

                    foreach (var id in rolesSeleccionadosIds.Except(actualesIds))
                        await _srv.AsignarRolUsuarioAsync(usuarioGuardado.Id, id);
                    foreach (var id in actualesIds.Except(rolesSeleccionadosIds))
                        await _srv.QuitarRolUsuarioAsync(usuarioGuardado.Id, id);
                }

                _dialogService.ShowInfo("Usuario guardado correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar usuario");
                _dialogService.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }

        [RelayCommand]
        public void Close() => this.CloseWindowOfViewModel();

        private bool PuedeAdministrarRoles() => EsEdicion && !IsBusy;

        [RelayCommand(CanExecute = nameof(PuedeAdministrarRoles))]
        private async Task AdministrarRolesAsync()
        {
            if (_entidad.Id == 0)
            {
                _dialogService.ShowError("Debe guardar el usuario primero antes de administrar sus roles.");
                return;
            }

            var ok = _dialogService.ShowDialog<UsuarioRolesViewModel>(vm => vm.CargarAsync(_entidad.Id, _entidad.NombreUsuario).Wait());
            if (ok == true)
                await CargarRolesAsync();
        }
    }
}