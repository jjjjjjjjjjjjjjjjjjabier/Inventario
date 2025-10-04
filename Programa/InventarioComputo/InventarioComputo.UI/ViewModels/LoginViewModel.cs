using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.UI.Extensions;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InventarioComputo.UI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ISessionService _session;
        private readonly IDialogService _dialog;

        [ObservableProperty]
        private string _titulo = "Iniciar Sesion";

        [ObservableProperty]
        private string _usuario = string.Empty;

        // Este valor se enlaza via PasswordBoxBehavior
        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _loginExitoso;

        public LoginViewModel(ISessionService session, IDialogService dialog, ILogger<LoginViewModel> logger)
        {
            _session = session;
            _dialog = dialog;
            Logger = logger;
        }

        [RelayCommand]
        private async Task IngresarAsync()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
            {
                _dialog.ShowError("Por favor, ingrese usuario y contrasena.");
                return;
            }

            IsBusy = true;
            try
            {
                var ok = await _session.IniciarSesionAsync(Usuario.Trim(), Password);
                if (!ok)
                {
                    _dialog.ShowError("Usuario o contrasena incorrectos, o usuario inactivo.");
                    return;
                }

                LoginExitoso = true;
                this.CloseWindowOfViewModel();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error en inicio de sesion");
                _dialog.ShowError("Ocurrio un error al iniciar sesion.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            LoginExitoso = false;
            this.CloseWindowOfViewModel();
        }
    }
}