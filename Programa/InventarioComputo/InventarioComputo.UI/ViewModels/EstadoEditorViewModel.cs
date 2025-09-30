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
    public partial class EstadoEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IEstadoService _srv;
        private readonly IDialogService _dialogService;
        private Estado _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nuevo Estado";

        public string Nombre
        {
            get => _entidad.Nombre;
            set => SetProperty(_entidad.Nombre, value, _entidad, (e, v) => e.Nombre = v);
        }

        public string? Descripcion
        {
            get => _entidad.Descripcion;
            set => SetProperty(_entidad.Descripcion, value, _entidad, (e, v) => e.Descripcion = v);
        }

        public string? ColorHex
        {
            get => _entidad.ColorHex;
            set => SetProperty(_entidad.ColorHex, value, _entidad, (e, v) => e.ColorHex = v);
        }

        public bool Activo
        {
            get => _entidad.Activo;
            set => SetProperty(_entidad.Activo, value, _entidad, (e, v) => e.Activo = v);
        }

        public bool DialogResult { get; set; }

        public EstadoEditorViewModel(IEstadoService srv, IDialogService dialogService, ILogger<EstadoEditorViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEstado(Estado estado)
        {
            _entidad = estado;
            if (estado.Id > 0)
            {
                Titulo = "Editar Estado";
            }
            OnPropertyChanged(nameof(Nombre));
            OnPropertyChanged(nameof(Descripcion));
            OnPropertyChanged(nameof(ColorHex));
            OnPropertyChanged(nameof(Activo));
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length > 100)
            {
                _dialogService.ShowError("El nombre es obligatorio y no debe exceder 100 caracteres.");
                return;
            }
            if (Descripcion?.Length > 255)
            {
                _dialogService.ShowError("La descripción no debe exceder 255 caracteres.");
                return;
            }
            if (string.IsNullOrWhiteSpace(ColorHex) || ColorHex.Length > 9)
            {
                _dialogService.ShowError("El color es obligatorio y no debe exceder 9 caracteres (#RRGGBB o #AARRGGBB).");
                return;
            }
            try
            {
                await _srv.GuardarAsync(_entidad);
                _dialogService.ShowInfo("Estado guardado correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar estado");
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