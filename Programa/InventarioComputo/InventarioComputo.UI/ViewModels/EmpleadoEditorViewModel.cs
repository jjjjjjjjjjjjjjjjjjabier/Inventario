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
        private readonly IDialogService _dialog;

        private Empleado _entidad = new();

        [ObservableProperty]
        private string _titulo = "Nuevo Empleado";

        public string NombreCompleto
        {
            get => _entidad.NombreCompleto;
            set => SetProperty(_entidad.NombreCompleto, value, _entidad, (e, v) => e.NombreCompleto = v);
        }

        public string? Correo
        {
            get => _entidad.Correo;
            set => SetProperty(_entidad.Correo, value, _entidad, (e, v) => e.Correo = v);
        }

        public string? Telefono
        {
            get => _entidad.Telefono;
            set => SetProperty(_entidad.Telefono, value, _entidad, (e, v) => e.Telefono = v);
        }

        public bool Activo
        {
            get => _entidad.Activo;
            set => SetProperty(_entidad.Activo, value, _entidad, (e, v) => e.Activo = v);
        }

        public bool DialogResult { get; set; }

        public EmpleadoEditorViewModel(IEmpleadoService srv, IDialogService dialog, ILogger<EmpleadoEditorViewModel> log)
        {
            _srv = srv;
            _dialog = dialog;
            Logger = log;
        }

        public void SetEntidad(Empleado entidad)
        {
            _entidad = entidad;
            Titulo = entidad.Id > 0 ? "Editar Empleado" : "Nuevo Empleado";
            OnPropertyChanged(nameof(NombreCompleto));
            OnPropertyChanged(nameof(Correo));
            OnPropertyChanged(nameof(Telefono));
            OnPropertyChanged(nameof(Activo));
        }

        [RelayCommand]
        public async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto) || NombreCompleto.Length > 200)
            {
                _dialog.ShowError("El nombre es obligatorio y no debe exceder 200 caracteres.");
                return;
            }
            if (Correo?.Length > 150)
            {
                _dialog.ShowError("El correo no debe exceder 150 caracteres.");
                return;
            }
            if (Telefono?.Length > 50)
            {
                _dialog.ShowError("El teléfono no debe exceder 50 caracteres.");
                return;
            }

            try
            {
                await _srv.GuardarAsync(_entidad);
                _dialog.ShowInfo("Empleado guardado correctamente.");
                DialogResult = true;
                this.CloseWindowOfViewModel();
            }
            catch (InvalidOperationException ex)
            {
                _dialog.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al guardar empleado");
                _dialog.ShowError("Ocurrió un error al guardar: " + ex.Message);
            }
        }

        [RelayCommand]
        public void Close() => this.CloseWindowOfViewModel();
    }
}