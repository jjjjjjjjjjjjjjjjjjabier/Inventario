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
    public partial class EstadoEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IEstadoService _srv;

        [ObservableProperty]
        private Estado? _entidad;

        public EstadoEditorViewModel(IEstadoService srv, ILogger<EstadoEditorViewModel> log)
        {
            _srv = srv;
            Logger = log;
        }

        public void SetEstado(Estado estado)
        {
            Entidad = new Estado
            {
                Id = estado.Id,
                Nombre = estado.Nombre,
                Descripcion = estado.Descripcion,
                ColorHex = estado.ColorHex,
                Activo = estado.Activo
            };
        }

        [RelayCommand]
        private async Task GuardarAsync(Window window)
        {
            if (Entidad == null || string.IsNullOrWhiteSpace(Entidad.Nombre))
            {
                ShowError("El nombre no puede estar vacío.");
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
                Logger?.LogError(ex, "Error al guardar estado.");
                ShowError($"No se pudo guardar el estado. Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}