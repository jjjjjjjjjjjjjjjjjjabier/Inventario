using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace InventarioComputo.UI.ViewModels
{
    public partial class TipoEquipoEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly ITipoEquipoService _srv;

        [ObservableProperty]
        private TipoEquipo? _entidad;

        public TipoEquipoEditorViewModel(ITipoEquipoService srv, ILogger<TipoEquipoEditorViewModel> log)
        {
            _srv = srv;
            Logger = log;
        }

        public void SetEntidad(TipoEquipo tipoEquipo)
        {
            Entidad = new TipoEquipo
            {
                Id = tipoEquipo.Id,
                Nombre = tipoEquipo.Nombre,
                Activo = tipoEquipo.Activo
            };
        }

        [RelayCommand]
        private async Task GuardarAsync(Window window)
        {
            if (Entidad == null || string.IsNullOrWhiteSpace(Entidad.Nombre))
            {
                ShowError("El nombre es obligatorio.");
                return;
            }

            IsBusy = true;
            try
            {
                await _srv.GuardarAsync(Entidad);
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar tipo de equipo.");
                ShowError($"No se pudo guardar. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}