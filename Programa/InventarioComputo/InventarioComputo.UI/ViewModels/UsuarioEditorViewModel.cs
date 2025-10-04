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

        [ObservableProperty]
        private string _titulo = "Nuevo Usuario";

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

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

        public UsuarioEditorViewModel(
            IUsuarioService srv,
            IRolService rolSrv,
            IDialogService dialogService,
            ILogger<UsuarioEditorViewModel> log)
        {
            _srv = srv;
            _rolSrv = rolSrv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEntidad(Usuario entidad)
        {
            _entidad = entidad;
            Titulo = entidad.Id > 0 ? "Editar Usuario" : "Nuevo Usuario";

            OnPropertyChanged(nameof(NombreUsuario));
            OnPropertyChanged(nameof(NombreCompleto));
            OnPropertyChanged(nameof(Activo));

            _ = CargarRolesAsync();
        }

        private async Task CargarRolesAsync(CancellationToken ct = default)
        {
            try
            {
                Roles.Clear();

                var rolesDisponibles = await _rolSrv.ObtenerTodosAsync(true, ct);
                var rolesUsuario = _entidad.Id > 0
                    ? await _srv.ObtenerRolesDeUsuarioAsync(_entidad.Id, ct)
                    : Array.Empty<Rol>();

                foreach (var rol in rolesDisponibles)
                {
                    var seleccionado = rolesUsuario.Any(r => r.Id == rol.Id);
                    Roles.Add(new RolCheckboxViewModel(rol.Id, rol.Nombre, seleccionado));
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando roles");
                _dialogService.ShowError("No se pudieron cargar los roles.");
            }
        }

        // Corrigiendo el método de validación de contraseña
        private bool ValidarContraseña(string password)
        {
            // Debe tener al menos 6 caracteres
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            // Validaciones básicas
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
                    if (Password.Length < 6)
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
                // Guardar usuario (si Password vacío en edición, no cambia)
                var usuarioGuardado = await _srv.GuardarAsync(_entidad, string.IsNullOrEmpty(Password) ? null : Password);

                // Sincronizar roles
                var rolesSeleccionadosIds = Roles.Where(r => r.IsSelected).Select(r => r.RolId).ToHashSet();
                var rolesActuales = await _srv.ObtenerRolesDeUsuarioAsync(usuarioGuardado.Id);
                var actualesIds = rolesActuales.Select(r => r.Id).ToHashSet();

                // Asignar nuevos
                foreach (var id in rolesSeleccionadosIds.Except(actualesIds))
                    await _srv.AsignarRolUsuarioAsync(usuarioGuardado.Id, id);

                // Quitar no seleccionados
                foreach (var id in actualesIds.Except(rolesSeleccionadosIds))
                    await _srv.QuitarRolUsuarioAsync(usuarioGuardado.Id, id);

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
        public void Close()
        {
            this.CloseWindowOfViewModel();
        }

        [ObservableProperty]
        private bool _esEmpleadoSolamente;

        public void SetUsuario(Usuario usuario)
        {
            _entidad = usuario;
            if (usuario.Id > 0)
            {
                Titulo = "Editar Usuario";
            }

            NombreUsuario = usuario.NombreUsuario;
            NombreCompleto = usuario.NombreCompleto;
            EsEmpleadoSolamente = usuario.EsEmpleadoSolamente;
            Activo = usuario.Activo;

            // Cargar roles si es necesario...
        }
    }
}