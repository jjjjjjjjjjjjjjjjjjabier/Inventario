using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Extensions;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class EmpleadoEditorViewModel : BaseViewModel, IEditorViewModel
    {
        private readonly IEmpleadoService _srv;
        private readonly IDialogService _dialogService;
        private Empleado _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nuevo Empleado";

        public string NombreCompleto
        {
            get => _entidad.NombreCompleto;
            set => SetProperty(_entidad.NombreCompleto, value, _entidad, (e, v) => e.NombreCompleto = v);
        }

        public string? Puesto
        {
            get => _entidad.Puesto;
            set => SetProperty(_entidad.Puesto, value, _entidad, (e, v) => e.Puesto = v);
        }

        public bool Activo
        {
            get => _entidad.Activo;
            set => SetProperty(_entidad.Activo, value, _entidad, (e, v) => e.Activo = v);
        }

        public bool DialogResult { get; set; }

        public EmpleadoEditorViewModel(IEmpleadoService srv, IDialogService dialogService, ILogger<EmpleadoEditorViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            Logger = log;
        }

        public void SetEmpleado(Empleado entidad)
        {
            _entidad = entidad;
            if (entidad.Id > 0)
            {
                Titulo = "Editar Empleado";
            }
            OnPropertyChanged(nameof(NombreCompleto));
            OnPropertyChanged(nameof(Puesto));
            OnPropertyChanged(nameof(Activo));
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto) || NombreCompleto.Length > 200)
            {
                _dialogService.ShowError("El nombre completo es obligatorio y debe tener menos de 200 caracteres.");
                return;
            }

            try
            {
                await _srv.GuardarAsync(_entidad);
                _dialogService.ShowInfo("Empleado guardado correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar el empleado");
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