using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class ZonaEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IZonaService _srv;
        private readonly IDialogService _dialogService;
        private Zona _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nueva Zona";

        public string Nombre
        {
            get => _entidad.Nombre;
            set => SetProperty(_entidad.Nombre, value, _entidad, (e, v) => e.Nombre = v);
        }

        public bool Activo
        {
            get => _entidad.Activo;
            set => SetProperty(_entidad.Activo, value, _entidad, (e, v) => e.Activo = v);
        }

        public bool DialogResult { get; set; }

        public ZonaEditorViewModel(IZonaService srv, IDialogService dialogService, ILogger<ZonaEditorViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEntidad(Zona entidad, Area areaPadre)
        {
            _entidad = entidad;
            if (entidad.Id > 0)
            {
                Titulo = $"Editar Zona en {areaPadre.Nombre}";
            }
            else
            {
                Titulo = $"Nueva Zona en {areaPadre.Nombre}";
            }
            OnPropertyChanged(nameof(Nombre));
            OnPropertyChanged(nameof(Activo));
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            try
            {
                await _srv.GuardarAsync(_entidad, default);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar la zona");
                _dialogService.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }
    }
}