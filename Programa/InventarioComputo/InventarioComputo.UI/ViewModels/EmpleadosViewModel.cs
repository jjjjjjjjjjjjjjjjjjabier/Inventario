using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class EmpleadosViewModel : BaseViewModel
    {
        private readonly IEmpleadoService _srv;
        private readonly IDialogService _dialogService;
        private readonly ISessionService _session;

        [ObservableProperty]
        private string _filtro = string.Empty;

        [ObservableProperty]
        private bool _mostrarInactivos;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private Empleado? _seleccionado;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
        [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
        private bool _esAdministrador;

        public ObservableCollection<Empleado> Empleados { get; } = new();

        public EmpleadosViewModel(IEmpleadoService srv, IDialogService dialogService, ISessionService session, ILogger<EmpleadosViewModel> log)
        {
            _srv = srv;
            _dialogService = dialogService;
            _session = session;
            Logger = log;

            EsAdministrador = _session.TieneRol("Administrador");
            _session.SesionCambiada += (s, logged) =>
            {
                EsAdministrador = _session.TieneRol("Administrador");
                CrearCommand?.NotifyCanExecuteChanged();
                EditarCommand?.NotifyCanExecuteChanged();
                EliminarCommand?.NotifyCanExecuteChanged();
            };
        }

        partial void OnMostrarInactivosChanged(bool value) => _ = BuscarAsync();
        partial void OnFiltroChanged(string value) => _ = BuscarAsync();

        [RelayCommand]
        public async Task LoadedAsync() => await BuscarAsync();

        [RelayCommand]
        private async Task BuscarAsync()
        {
            IsBusy = true;
            try
            {
                Empleados.Clear();
                var lista = await _srv.BuscarAsync(string.IsNullOrWhiteSpace(Filtro) ? null : Filtro, MostrarInactivos);
                foreach (var e in lista) Empleados.Add(e);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error buscando empleados");
                _dialogService.ShowError("No se pudieron cargar los empleados.");
            }
            finally { IsBusy = false; }
        }

        private bool PuedeCrear() => EsAdministrador && !IsBusy;
        private bool PuedeEditarEliminar() => EsAdministrador && Seleccionado != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(PuedeCrear))]
        public void Crear()
        {
            var nuevo = new Empleado { Activo = true };
            _dialogService.ShowDialog<EmpleadoEditorViewModel>(vm => vm.SetEntidad(nuevo));
            _ = BuscarAsync();
        }

        [RelayCommand(CanExecute = nameof(PuedeEditarEliminar))]
        public void Editar()
        {
            if (Seleccionado == null) return;

            var copia = new Empleado
            {
                Id = Seleccionado.Id,
                NombreCompleto = Seleccionado.NombreCompleto,
                Correo = Seleccionado.Correo,
                Telefono = Seleccionado.Telefono,
                Activo = Seleccionado.Activo
            };
            _dialogService.ShowDialog<EmpleadoEditorViewModel>(vm => vm.SetEntidad(copia));
            _ = BuscarAsync();
        }

        [RelayCommand(CanExecute = nameof(PuedeEditarEliminar))]
        public async Task EliminarAsync()
        {
            if (Seleccionado == null) return;
            if (!_dialogService.Confirm($"¿Desactivar al empleado '{Seleccionado.NombreCompleto}'?", "Confirmar")) return;

            IsBusy = true;
            try
            {
                await _srv.EliminarAsync(Seleccionado.Id);
                await BuscarAsync();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error desactivando empleado");
                _dialogService.ShowError("No se pudo desactivar el empleado. Es posible que esté en uso.");
            }
            finally { IsBusy = false; }
        }
    }
}