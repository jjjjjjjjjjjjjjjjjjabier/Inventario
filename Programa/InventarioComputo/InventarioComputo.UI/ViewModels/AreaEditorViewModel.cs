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
    public partial class AreaEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IAreaService _srv;

        [ObservableProperty]
        private Area? _entidad;

        [ObservableProperty]
        private Sede? _sedePadre;

        public AreaEditorViewModel(IAreaService srv, ILogger<AreaEditorViewModel> log)
        {
            _srv = srv;
            Logger = log;
        }

        public void SetEntidad(Area area, Sede sedePadre)
        {
            Entidad = new Area
            {
                Id = area.Id,
                Nombre = area.Nombre,
                SedeId = area.SedeId,
                Activo = area.Activo
            };
            SedePadre = sedePadre;
        }

        [RelayCommand]
        private async Task GuardarAsync(Window window)
        {
            if (Entidad == null || SedePadre == null || string.IsNullOrWhiteSpace(Entidad.Nombre))
            {
                ShowError("El nombre es obligatorio.");
                return;
            }

            IsBusy = true;
            try
            {
                Entidad.SedeId = SedePadre.Id;
                await _srv.GuardarAsync(Entidad, default);
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar área.");
                ShowError($"No se pudo guardar el área. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}