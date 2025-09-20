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
    public partial class AreaEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IAreaService _srv;
        private readonly IDialogService _dialogService;

        private Area _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nueva Área";

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

        public AreaEditorViewModel(IAreaService srv, IDialogService dialogService, ILogger<AreaEditorViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEntidad(Area entidad, Sede sedePadre)
        {
            _entidad = entidad;
            if (entidad.Id > 0)
            {
                Titulo = $"Editar Área en {sedePadre.Nombre}";
            }
            else
            {
                Titulo = $"Nueva Área en {sedePadre.Nombre}";
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
                Logger?.LogError(ex, "Error al guardar el área");
                _dialogService.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }
    }
}