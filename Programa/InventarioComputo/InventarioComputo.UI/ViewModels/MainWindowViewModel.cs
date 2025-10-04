using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts; // ISessionService
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private readonly EmpleadosViewModel _empleadosVm;
        private readonly EquiposComputoViewModel _equiposVm;
        private readonly EstadosViewModel _estadosVm;
        private readonly UnidadesViewModel _unidadesViewModel;
        private readonly UbicacionesViewModel _ubicacionesViewModel;
        private readonly TiposEquipoViewModel _tiposEquipoViewModel;
        private readonly ReportesViewModel _reportesViewModel;
        private readonly ISessionService _sessionService;
        private readonly IDialogService _dialogService;
        private readonly UsuariosViewModel _usuariosViewModel;

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        [ObservableProperty]
        private bool _esAdministrador;

        public MainWindowViewModel(
            EmpleadosViewModel empleadosVm,
            EquiposComputoViewModel equiposVm,
            EstadosViewModel estadosVm,
            UnidadesViewModel unidadesViewModel,
            UbicacionesViewModel ubicacionesViewModel,
            TiposEquipoViewModel tiposEquipoViewModel,
            ReportesViewModel reportesViewModel,
            ISessionService sessionService,
            IDialogService dialogService,
            UsuariosViewModel usuariosViewModel)
        {
            _empleadosVm = empleadosVm;
            _equiposVm = equiposVm;
            _estadosVm = estadosVm;
            _unidadesViewModel = unidadesViewModel;
            _ubicacionesViewModel = ubicacionesViewModel;
            _tiposEquipoViewModel = tiposEquipoViewModel;
            _reportesViewModel = reportesViewModel;
            _sessionService = sessionService;
            _dialogService = dialogService;
            _usuariosViewModel = usuariosViewModel;

            ConfigurarInfoUsuario();
            CurrentView = _equiposVm; // El DataTemplate resuelve la vista
        }

        private void ConfigurarInfoUsuario()
        {
            if (_sessionService.UsuarioActual != null)
            {
                NombreUsuario = _sessionService.UsuarioActual.NombreCompleto;
                EsAdministrador = _sessionService.TieneRol("Administrador");
            }
        }

        [RelayCommand] private Task NavigateToEquiposComputo() { CurrentView = _equiposVm; return Task.CompletedTask; }
        [RelayCommand] private Task NavigateToEstados() { CurrentView = _estadosVm; return Task.CompletedTask; }
        [RelayCommand] private Task NavigateToUnidades() { CurrentView = _unidadesViewModel; return Task.CompletedTask; }
        [RelayCommand] private Task NavigateToUbicaciones() { CurrentView = _ubicacionesViewModel; return Task.CompletedTask; }
        [RelayCommand] private Task NavigateToTiposEquipo() { CurrentView = _tiposEquipoViewModel; return Task.CompletedTask; }
        [RelayCommand] private Task NavigateToReportes() { CurrentView = _reportesViewModel; return Task.CompletedTask; }
        [RelayCommand] private Task NavigateToUsuarios() { CurrentView = _usuariosViewModel; return Task.CompletedTask; }
        [RelayCommand] private void CerrarSesion() => _sessionService.CerrarSesion();
        [RelayCommand] public void MostrarEmpleados() { CurrentView = _empleadosVm; }
        [RelayCommand] public void MostrarEquipos() { CurrentView = _equiposVm; }
        [RelayCommand] public void MostrarEstados() { CurrentView = _estadosVm; }
    }
}