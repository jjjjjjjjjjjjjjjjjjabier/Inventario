using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly EquiposComputoViewModel _equiposComputoViewModel;
        private readonly EstadosViewModel _estadosViewModel;
        private readonly UnidadesViewModel _unidadesViewModel;
        private readonly UbicacionesViewModel _ubicacionesViewModel;
        private readonly TiposEquipoViewModel _tiposEquipoViewModel;
        private readonly ReportesViewModel _reportesViewModel;
        private readonly ISessionService _sessionService;
        private readonly IDialogService _dialogService;
        private readonly UsuariosViewModel _usuariosViewModel;

        [ObservableProperty]
        private BaseViewModel? _currentViewModel;

        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        [ObservableProperty]
        private bool _esAdministrador;

        [ObservableProperty]
        private string _pageTitle = "Inventario de Equipos";

        public MainWindowViewModel(
            IServiceProvider serviceProvider,
            EquiposComputoViewModel equiposComputoViewModel,
            EstadosViewModel estadosViewModel,
            UnidadesViewModel unidadesViewModel,
            UbicacionesViewModel ubicacionesViewModel,
            TiposEquipoViewModel tiposEquipoViewModel,
            ReportesViewModel reportesViewModel,
            ISessionService sessionService,
            IDialogService dialogService,
            UsuariosViewModel usuariosViewModel)
        {
            _serviceProvider = serviceProvider;

            _equiposComputoViewModel = equiposComputoViewModel;
            _estadosViewModel = estadosViewModel;
            _unidadesViewModel = unidadesViewModel;
            _ubicacionesViewModel = ubicacionesViewModel;
            _tiposEquipoViewModel = tiposEquipoViewModel;
            _reportesViewModel = reportesViewModel;
            _sessionService = sessionService;
            _dialogService = dialogService;
            _usuariosViewModel = usuariosViewModel;

            ConfigurarInfoUsuario();

            CurrentViewModel = _equiposComputoViewModel;
            PageTitle = "Inventario de Equipos";
        }

        private void ConfigurarInfoUsuario()
        {
            if (_sessionService.UsuarioActual != null)
            {
                NombreUsuario = _sessionService.UsuarioActual.NombreCompleto;
                EsAdministrador = _sessionService.TieneRol("Administrador");
            }
        }

        [RelayCommand]
        private Task NavigateToEquiposComputo()
        {
            CurrentViewModel = _equiposComputoViewModel;
            PageTitle = "Inventario de Equipos";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToEstados()
        {
            CurrentViewModel = _estadosViewModel;
            PageTitle = "Gestión de Estados";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToUnidades()
        {
            CurrentViewModel = _unidadesViewModel;
            PageTitle = "Gestión de Unidades";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToUbicaciones()
        {
            CurrentViewModel = _ubicacionesViewModel;
            PageTitle = "Gestión de Ubicaciones";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToTiposEquipo()
        {
            CurrentViewModel = _tiposEquipoViewModel;
            PageTitle = "Tipos de Equipo";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToReportes()
        {
            CurrentViewModel = _reportesViewModel;
            PageTitle = "Reportes";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToUsuarios()
        {
            CurrentViewModel = _usuariosViewModel;
            PageTitle = "Administración de Usuarios";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToEmpleados()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<EmpleadosViewModel>();
            PageTitle = "Gestión de Empleados";
            return Task.CompletedTask;
        }

        [RelayCommand]
        private void CerrarSesion()
        {
            _sessionService.CerrarSesion();
        }
    }
}