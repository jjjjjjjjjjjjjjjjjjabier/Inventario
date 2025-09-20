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
    public partial class UnidadesViewModel : BaseViewModel
    {
        private readonly IUnidadService _srv;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Unidad? _unidadSeleccionada;

        public ObservableCollection<Unidad> Unidades { get; } = new();

        public UnidadesViewModel(IUnidadService srv, IDialogService dialogService, ILogger<UnidadesViewModel> log)
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
                Unidades.Clear();
                var lista = await _srv.BuscarAsync(null, true);
                foreach (var item in lista) Unidades.Add(item);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando unidades");
                _dialogService.ShowError("Ocurrió un error al cargar las unidades.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CrearAsync()
        {
            var nuevo = new Unidad { Activo = true };
            if (_dialogService.ShowDialog<UnidadEditorViewModel>(vm => vm.SetEntidad(nuevo)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool CanEditarEliminar() => UnidadSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (UnidadSeleccionada == null) return;
            if (_dialogService.ShowDialog<UnidadEditorViewModel>(vm => vm.SetEntidad(UnidadSeleccionada)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (UnidadSeleccionada == null) return;
            if (!_dialogService.Confirm($"¿Eliminar la unidad '{UnidadSeleccionada.Nombre}'?", "Confirmar Eliminación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(UnidadSeleccionada.Id);
                Unidades.Remove(UnidadSeleccionada);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar unidad");
                _dialogService.ShowError("No se pudo eliminar la unidad. Es posible que esté en uso.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}