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
        private async Task LoadedAsync()
        {
            await BuscarAsync();
        }

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Unidades.Clear();
                var unidades = await _srv.BuscarAsync(null, true);
                foreach (var unidad in unidades)
                {
                    Unidades.Add(unidad);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al buscar unidades");
                ShowError("Ocurrió un error al cargar las unidades.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CrearAsync()
        {
            var nuevaUnidad = new Unidad { Activo = true };
            if (_dialogService.ShowDialog<UnidadEditorViewModel>(vm => vm.SetUnidad(nuevaUnidad)) == true)
            {
                await BuscarAsync();
            }
        }

        private bool CanEditarEliminar() => UnidadSeleccionada != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EditarAsync()
        {
            if (UnidadSeleccionada == null) return;
            if (_dialogService.ShowDialog<UnidadEditorViewModel>(vm => vm.SetUnidad(UnidadSeleccionada)) == true)
            {
                await BuscarAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditarEliminar))]
        private async Task EliminarAsync()
        {
            if (UnidadSeleccionada == null) return;
            if (!ConfirmAction($"¿Realmente desea eliminar la unidad '{UnidadSeleccionada.Nombre}'?", "Confirmar Eliminación")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(UnidadSeleccionada.Id);
                Unidades.Remove(UnidadSeleccionada);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al eliminar la unidad con ID {UnidadId}", UnidadSeleccionada.Id);
                ShowError("Ocurrió un error al eliminar la unidad. Es posible que esté en uso.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}