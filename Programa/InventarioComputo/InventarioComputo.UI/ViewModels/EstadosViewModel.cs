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
    public partial class EstadosViewModel : BaseViewModel
    {
        private readonly IEstadoService _srv;
        private readonly IDialogService _dialogService;

        // **LA CORRECCIÓN MÁS IMPORTANTE ESTÁ AQUÍ**
        // Se usan strings con los nombres exactos de los comandos generados por el Toolkit.
        [ObservableProperty]
        [NotifyCanExecuteChangedFor("EditarAsyncCommand")]
        [NotifyCanExecuteChangedFor("EliminarAsyncCommand")]
        private Estado? _estadoSeleccionado;

        [ObservableProperty]
        private bool _mostrarInactivos;

        public ObservableCollection<Estado> Estados { get; } = new();

        public EstadosViewModel(IEstadoService srv, IDialogService dialogService, ILogger<EstadosViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();

        [RelayCommand]
        private async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Estados.Clear();
                var lista = await _srv.BuscarAsync(null, MostrarInactivos);
                foreach (var e in lista) Estados.Add(e);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al buscar estados");
                _dialogService.ShowError("Ocurrió un error al cargar los estados.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CrearAsync()
        {
            var nuevo = new Estado { Activo = true, ColorHex = "#FFFFFF" };
            if (_dialogService.ShowDialog<EstadoEditorViewModel>(vm => vm.SetEstado(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool CanEditarEliminar() => EstadoSeleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (EstadoSeleccionado == null) return;
            if (_dialogService.ShowDialog<EstadoEditorViewModel>(vm => vm.SetEstado(EstadoSeleccionado)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (EstadoSeleccionado == null) return;
            if (!_dialogService.Confirm($"¿Está seguro de querer desactivar el estado '{EstadoSeleccionado.Nombre}'?", "Confirmar Desactivación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(EstadoSeleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar estado");
                _dialogService.ShowError("No se pudo desactivar el estado. Es posible que esté en uso.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}