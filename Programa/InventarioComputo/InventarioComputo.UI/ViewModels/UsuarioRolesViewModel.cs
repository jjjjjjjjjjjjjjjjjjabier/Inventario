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
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class UsuarioRolesViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string _titulo = "Gestionar Roles";

        [ObservableProperty]
        private int _usuarioId;

        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        public ObservableCollection<RolCheckboxViewModel> Roles { get; } = new();

        public bool DialogResult { get; set; }

        public UsuarioRolesViewModel(
            IUsuarioService usuarioService,
            IRolService rolService,
            IDialogService dialogService,
            ILogger<UsuarioRolesViewModel> log)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _dialogService = dialogService;
            Logger = log;
        }

        public async Task CargarAsync(int usuarioId, string nombreUsuario)
        {
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
            Titulo = $"Gestionar Roles - {nombreUsuario}";

            await CargarRolesAsync();
        }

        private async Task CargarRolesAsync()
        {
            IsBusy = true;
            try
            {
                // Obtener todos los roles disponibles
                var todosLosRoles = await _rolService.ObtenerTodosAsync(true);

                // Obtener los roles actuales del usuario
                var rolesUsuario = await _usuarioService.ObtenerRolesDeUsuarioAsync(UsuarioId);

                Roles.Clear();
                foreach (var rol in todosLosRoles)
                {
                    // Verificar si el usuario tiene este rol
                    bool tieneRol = rolesUsuario.Any(r => r.Id == rol.Id);
                    Roles.Add(new RolCheckboxViewModel(rol.Id, rol.Nombre, tieneRol));
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar roles para el usuario {UsuarioId}", UsuarioId);
                _dialogService.ShowError("No se pudieron cargar los roles disponibles.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (!Roles.Any(r => r.IsSelected))
            {
                _dialogService.ShowError("El usuario debe tener al menos un rol asignado.");
                return;
            }

            IsBusy = true;
            try
            {
                // Obtener roles actuales
                var rolesActuales = await _usuarioService.ObtenerRolesDeUsuarioAsync(UsuarioId);

                // Para cada rol en la lista de checkboxes
                foreach (var rolVM in Roles)
                {
                    bool tieneRol = rolesActuales.Any(r => r.Id == rolVM.RolId);

                    // Si el rol está seleccionado y el usuario no lo tiene, asignarlo
                    if (rolVM.IsSelected && !tieneRol)
                        await _usuarioService.AsignarRolUsuarioAsync(UsuarioId, rolVM.RolId);

                    // Si el rol no está seleccionado pero el usuario lo tiene, quitarlo
                    else if (!rolVM.IsSelected && tieneRol)
                        await _usuarioService.QuitarRolUsuarioAsync(UsuarioId, rolVM.RolId);
                }

                _dialogService.ShowInfo("Roles actualizados correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al actualizar roles para el usuario {UsuarioId}", UsuarioId);
                _dialogService.ShowError("Ocurrió un error al actualizar los roles del usuario.");
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
            this.CloseWindowOfViewModel();
        }
    }
}