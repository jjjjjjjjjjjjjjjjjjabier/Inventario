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

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Estado? _estadoSeleccionado;

        public ObservableCollection<Estado> Estados { get; } = new();

        public EstadosViewModel(IEstadoService srv, IDialogService dialogService, ILogger<EstadosViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        [RelayCommand]
        private async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Estados.Clear();
                var lista = await _srv.BuscarAsync(null, true);
                foreach (var e in lista) Estados.Add(e);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al buscar estados");
                ShowError("Ocurrió un error al cargar los estados.");
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
            if (!ConfirmAction($"¿Eliminar el estado '{EstadoSeleccionado.Nombre}'?", "Confirmar Eliminación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(EstadoSeleccionado.Id);
                Estados.Remove(EstadoSeleccionado);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar estado");
                ShowError("No se pudo eliminar el estado. Es posible que esté en uso.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}