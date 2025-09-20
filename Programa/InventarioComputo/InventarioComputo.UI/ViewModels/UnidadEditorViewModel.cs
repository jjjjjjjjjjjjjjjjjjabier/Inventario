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
    public partial class UnidadEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IUnidadService _srv;

        [ObservableProperty]
        private Unidad? _entidad;

        public UnidadEditorViewModel(IUnidadService srv, ILogger<UnidadEditorViewModel> log)
        {
            _srv = srv;
            Logger = log;
        }

        public void SetUnidad(Unidad unidad)
        {
            Entidad = new Unidad
            {
                Id = unidad.Id,
                Nombre = unidad.Nombre,
                Abreviatura = unidad.Abreviatura,
                Activo = unidad.Activo
            };
        }

        [RelayCommand]
        private async Task GuardarAsync(Window window)
        {
            if (Entidad == null || string.IsNullOrWhiteSpace(Entidad.Nombre) || string.IsNullOrWhiteSpace(Entidad.Abreviatura))
            {
                ShowError("El nombre y la abreviatura son obligatorios.");
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
                Logger?.LogError(ex, "Error al guardar la unidad");
                ShowError($"No se pudo guardar la unidad. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}