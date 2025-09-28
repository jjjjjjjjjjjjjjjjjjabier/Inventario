using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Extensions;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 100)
            {
                _dialogService.ShowError("El nombre de la zona es obligatorio y debe tener menos de 100 caracteres.");
                return;
            }
            try
            {
                await _srv.GuardarAsync(_entidad, default);
                _dialogService.ShowInfo("Zona guardada correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar zona");
                _dialogService.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }

        [RelayCommand]
        public void Close()
        {
                            this.CloseWindowOfViewModel();
        }
    }
}