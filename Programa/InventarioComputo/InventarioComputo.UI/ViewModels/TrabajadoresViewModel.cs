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
    public partial class TrabajadoresViewModel : BaseViewModel, IDisposable
    {
        private readonly IDialogService _dialogService;
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Usuario? _trabajadorSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        private string _filtroTexto = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private bool _esAdministrador;

        public ObservableCollection<Usuario> Trabajadores { get; } = new();

        public TrabajadoresViewModel(
            IDialogService dialogService,
            ISessionService sessionService,
            ILogger<TrabajadoresViewModel> log)
        {
            _dialogService = dialogService;
            _sessionService = sessionService;
            Logger = log;

            EsAdministrador = _sessionService.TieneRol("Administrador");
            _sessionService.SesionCambiada += OnSesionCambiada;
        }

        private void OnSesionCambiada(object? sender, bool estaLogueado)
        {
            EsAdministrador = _sessionService.TieneRol("Administrador");
            ActualizarCanExecute();
        }

        private void ActualizarCanExecute()
        {
            CrearCommand?.NotifyCanExecuteChanged();
            EditarCommand?.NotifyCanExecuteChanged();
            EliminarCommand?.NotifyCanExecuteChanged();
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();
        partial void OnFiltroTextoChanged(string value) => _ = BuscarAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Trabajadores.Clear();
                // Aquí implementarías la lógica para buscar trabajadores
                // Por ahora solo mostraremos datos de ejemplo
                Trabajadores.Add(new Usuario { Id = 1, NombreUsuario = "trabajador1", NombreCompleto = "Trabajador 1", Activo = true });
                Trabajadores.Add(new Usuario { Id = 2, NombreUsuario = "trabajador2", NombreCompleto = "Trabajador 2", Activo = true });
                
                await Task.CompletedTask; // Placeholder para operación asíncrona real
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando trabajadores");
                _dialogService.ShowError("Ocurrió un error al cargar los trabajadores.");
            }
            finally
            {
                IsBusy = false;
                ActualizarCanExecute();
            }
        }

        [RelayCommand(CanExecute = nameof(PuedeCrearEditar))]
        private void Crear()
        {
            _dialogService.ShowInfo("Funcionalidad no implementada: Crear trabajador");
        }

        private bool PuedeCrearEditar() => EsAdministrador && !IsBusy;

        private bool CanEditarEliminar() => EsAdministrador && TrabajadorSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private void Editar()
        {
            _dialogService.ShowInfo("Funcionalidad no implementada: Editar trabajador");
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private void Eliminar()
        {
            _dialogService.ShowInfo("Funcionalidad no implementada: Eliminar trabajador");
        }

        public void Dispose()
        {
            _sessionService.SesionCambiada -= OnSesionCambiada;
        }
    }
}