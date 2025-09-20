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
    public partial class SedeEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly ISedeService _srv;

        [ObservableProperty]
        private Sede? _entidad;

        public SedeEditorViewModel(ISedeService srv, ILogger<SedeEditorViewModel> log)
        {
            _srv = srv;
            Logger = log;
        }

        public void SetEntidad(Sede sede)
        {
            Entidad = new Sede
            {
                Id = sede.Id,
                Nombre = sede.Nombre,
                Activo = sede.Activo
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
                Logger?.LogError(ex, "Error al guardar sede.");
                ShowError($"No se pudo guardar la sede. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}