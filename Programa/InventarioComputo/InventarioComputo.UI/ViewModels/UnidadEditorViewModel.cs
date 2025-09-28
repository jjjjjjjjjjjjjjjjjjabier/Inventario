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
    public partial class UnidadEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IUnidadService _srv;
        private readonly IDialogService _dialogService;
        private Unidad _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nueva Unidad";

        public string Nombre
        {
            get => _entidad.Nombre;
            set => SetProperty(_entidad.Nombre, value, _entidad, (e, v) => e.Nombre = v);
        }

        public string Abreviatura
        {
            get => _entidad.Abreviatura;
            set => SetProperty(_entidad.Abreviatura, value, _entidad, (e, v) => e.Abreviatura = v);
        }

        public bool Activo
        {
            get => _entidad.Activo;
            set => SetProperty(_entidad.Activo, value, _entidad, (e, v) => e.Activo = v);
        }

        public bool DialogResult { get; set; }

        public UnidadEditorViewModel(IUnidadService srv, IDialogService dialogService, ILogger<UnidadEditorViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEntidad(Unidad entidad)
        {
            _entidad = entidad;
            if (entidad.Id > 0)
            {
                Titulo = "Editar Unidad";
            }
            OnPropertyChanged(nameof(Nombre));
            OnPropertyChanged(nameof(Abreviatura));
            OnPropertyChanged(nameof(Activo));
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 50)
            {
                _dialogService.ShowError("El nombre es obligatorio y debe tener menos de 50 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Abreviatura) || Abreviatura.Length > 10)
            {
                _dialogService.ShowError("La abreviatura es obligatoria y debe tener menos de 10 caracteres.");
                return;
            }
            try
            {
                if (await _srv.ExisteNombreAsync(Nombre, _entidad.Id))
                {
                    _dialogService.ShowError("Ya existe una unidad con ese nombre.");
                    return;
                }
                if (await _srv.ExisteAbreviaturaAsync(Abreviatura, _entidad.Id))
                {
                    _dialogService.ShowError("Ya existe una unidad con esa abreviatura.");
                    return;
                }
                await _srv.GuardarAsync(_entidad);
                _dialogService.ShowInfo("Unidad guardada correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar la unidad");
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