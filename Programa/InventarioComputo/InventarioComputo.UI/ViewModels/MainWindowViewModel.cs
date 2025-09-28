using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace InventarioComputo.UI.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
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
        private string _nombreUsuario;

        [ObservableProperty]
        private bool _esAdministrador;

        public MainWindowViewModel(
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
            _equiposComputoViewModel = equiposComputoViewModel;
            _estadosViewModel = estadosViewModel;
            _unidadesViewModel = unidadesViewModel;
            _ubicacionesViewModel = ubicacionesViewModel;
            _tiposEquipoViewModel = tiposEquipoViewModel;
            _reportesViewModel = reportesViewModel;
            _sessionService = sessionService;
            _dialogService = dialogService;
            _usuariosViewModel = usuariosViewModel;

            // Configurar información del usuario
            ConfigurarInfoUsuario();

            // Vista inicial (la propia vista ejecutará LoadedCommand)
            CurrentViewModel = _equiposComputoViewModel;
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
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToEstados()
        {
            CurrentViewModel = _estadosViewModel;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToUnidades()
        {
            CurrentViewModel = _unidadesViewModel;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToUbicaciones()
        {
            CurrentViewModel = _ubicacionesViewModel;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToTiposEquipo()
        {
            CurrentViewModel = _tiposEquipoViewModel;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToReportes()
        {
            CurrentViewModel = _reportesViewModel;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task NavigateToUsuarios()
        {
            CurrentViewModel = _usuariosViewModel;
            return Task.CompletedTask;
        }

        [RelayCommand]
        private void CerrarSesion()
        {
            _sessionService.CerrarSesion();
            
            // Cerrar la ventana principal
            var mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow.Show();
            // Mostrar ventana de login
            var loginView = new Views.LoginView();
            loginView.Show();
            
            // Cerrar la ventana principal
            mainWindow.Close();
        }
    }
}