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
    public partial class ZonaEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IZonaService _srv;

        [ObservableProperty]
        private Zona? _entidad;

        [ObservableProperty]
        private Area? _areaPadre;

        public ZonaEditorViewModel(IZonaService srv, ILogger<ZonaEditorViewModel> log)
        {
            _srv = srv;
            Logger = log;
        }

        public void SetEntidad(Zona zona, Area areaPadre)
        {
            Entidad = new Zona
            {
                Id = zona.Id,
                Nombre = zona.Nombre,
                AreaId = zona.AreaId,
                Activo = zona.Activo
            };
            AreaPadre = areaPadre;
        }

        [RelayCommand]
        private async Task GuardarAsync(Window window)
        {
            if (Entidad == null || AreaPadre == null || string.IsNullOrWhiteSpace(Entidad.Nombre))
            {
                ShowError("El nombre es obligatorio.");
                return;
            }

            IsBusy = true;
            try
            {
                Entidad.AreaId = AreaPadre.Id;
                await _srv.GuardarAsync(Entidad, default);
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar la zona.");
                ShowError($"No se pudo guardar la zona. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}