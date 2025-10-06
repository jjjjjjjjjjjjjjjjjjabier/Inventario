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
    public partial class UsuariosViewModel : BaseViewModel
    {
        private readonly IUsuarioService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Usuario? _usuarioSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        private string _filtro = string.Empty;

        [ObservableProperty]
        private bool _esAdministrador;

        public ObservableCollection<Usuario> Usuarios { get; } = new();

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

            EsAdministrador = _sessionService.TieneRol("Administrador");
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
                var lista = await _srv.BuscarAsync(filtro, MostrarInactivos);
                foreach (var item in lista) Usuarios.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando usuarios");
                _dialogService.ShowError("Ocurrió un error al cargar los usuarios.");
            }
            finally
            {
                IsBusy = false;
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
                Activo = UsuarioSeleccionado.Activo
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
            }
        }
    }
}