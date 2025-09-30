using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InventarioComputo.UI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;
        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;

        [ObservableProperty] private string _usuario = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private bool _loginInProgress;

        public bool LoginExitoso { get; private set; }

        public LoginViewModel(
            IAuthService authService,
            ISessionService sessionService,
            IDialogService dialogService,
            ILogger<LoginViewModel> logger)
        {
            _authService = authService;
            _sessionService = sessionService;
            _dialogService = dialogService;
            Logger = logger;
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            var usuario = (Usuario ?? string.Empty).Trim();
            var password = Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                _dialogService.ShowError("Por favor, ingrese usuario y contraseña.");
                return;
            }

            LoginInProgress = true;

            try
            {
                var ok = await _sessionService.IniciarSesionAsync(usuario, password);
                if (ok)
                {
                    LoginExitoso = true;

                    var window = System.Windows.Application.Current.Windows.OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this);

                    window?.Close();
                }
                else
                {
                    _dialogService.ShowError("Usuario o contraseña incorrectos.");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al iniciar sesión");
                _dialogService.ShowError("Ocurrió un error al intentar iniciar sesión. Inténtelo de nuevo.");
            }
            finally
            {
                LoginInProgress = false;
            }
        }

        [RelayCommand]
        public void Cancelar()
        {
            LoginExitoso = false;
            var window = System.Windows.Application.Current.Windows.OfType<Window>()
                .SingleOrDefault(w => w.DataContext == this);

            window?.Close();
        }
    }
}