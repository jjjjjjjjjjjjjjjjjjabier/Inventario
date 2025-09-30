using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;
using InventarioComputo.Infrastructure.Repositories;
using InventarioComputo.Infrastructure.Security;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels;
using InventarioComputo.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;
using System.Windows.Threading;

namespace InventarioComputo.UI
{
    public partial class App : System.Windows.Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .UseSerilog((host, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(host.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.File("Logs/inventario_log-.txt", rollingInterval: RollingInterval.Day)
                        .WriteTo.Console();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    var env = context.HostingEnvironment.EnvironmentName;
                    config.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<InventarioDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Seguridad
                    services.AddSingleton<IPasswordHasher, PasswordHasher>();

                    // Repos
                    services.AddScoped<ISedeRepository, SedeRepository>();
                    services.AddScoped<IAreaRepository, AreaRepository>();
                    services.AddScoped<IZonaRepository, ZonaRepository>
                    ();
                    services.AddScoped<IEstadoRepository, EstadoRepository>();
                    services.AddScoped<IUnidadRepository, UnidadRepository>();
                    services.AddScoped<ITipoEquipoRepository, TipoEquipoRepository>();
                    services.AddScoped<IEquipoComputoRepository, EquipoComputoRepository>();
                    services.AddScoped<IUsuarioRepository, UsuarioRepository>();
                    services.AddScoped<IRolRepository, RolRepository>();

                    // Repositorio para Historial
                    services.AddScoped<IHistorialMovimientoRepository, HistorialMovimientoRepository>();

                    // Servicios de aplicación
                    services.AddScoped<ISedeService, SedeService>();
                    services.AddScoped<IAreaService, AreaService>();
                    services.AddScoped<IZonaService, ZonaService>();
                    services.AddScoped<IEstadoService, EstadoService>();
                    services.AddScoped<IUnidadService, UnidadService>();
                    services.AddScoped<ITipoEquipoService, TipoEquipoService>();
                    services.AddScoped<IEquipoComputoService, EquipoComputoService>();
                    services.AddScoped<IAuthService, AuthService>();
                    services.AddScoped<ISessionService, SessionService>();
                    services.AddScoped<IUsuarioService, UsuarioService>();
                    services.AddScoped<IReporteService, ReporteService>();
                    services.AddScoped<IMovimientoService, MovimientoService>();
                    services.AddScoped<IRolService, RolService>();
                    services.AddScoped<IBitacoraService, InventarioComputo.Infrastructure.Services.BitacoraService>();

                    // Servicios de la UI
                    services.AddSingleton<DialogService>();
                    services.AddSingleton<IDialogService>(sp => sp.GetRequiredService<DialogService>());

                    // ViewModels
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<EstadosViewModel>();
                    services.AddTransient<EstadoEditorViewModel>();
                    services.AddTransient<UnidadesViewModel>();
                    services.AddTransient<UnidadEditorViewModel>();
                    services.AddTransient<TiposEquipoViewModel>();
                    services.AddTransient<TipoEquipoEditorViewModel>();
                    services.AddTransient<UbicacionesViewModel>();
                    services.AddTransient<SedeEditorViewModel>();
                    services.AddTransient<AreaEditorViewModel>();
                    services.AddTransient<ZonaEditorViewModel>();
                    services.AddTransient<EquiposComputoViewModel>();
                    services.AddTransient<EquipoComputoEditorViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<AsignarEquipoViewModel>();
                    services.AddTransient<HistorialEquipoViewModel>();
                    services.AddTransient<ReportesViewModel>();
                    services.AddTransient<UsuariosViewModel>();
                    services.AddTransient<UsuarioEditorViewModel>();
                    services.AddTransient<UsuarioRolesViewModel>();

                    // Services
                    services.AddTransient<EstadoEditorView>();
                    services.AddTransient<UnidadEditorView>();
                    services.AddTransient<TipoEquipoEditorView>();
                    services.AddTransient<SedeEditorView>();
                    services.AddTransient<AreaEditorView>();
                    services.AddTransient<ZonaEditorView>();
                    services.AddTransient<EquipoComputoEditorView>();
                    services.AddTransient<LoginView>();
                    services.AddTransient<MainWindow>();
                    services.AddTransient<AsignarEquipoView>();
                    services.AddTransient<HistorialEquipoView>();
                    services.AddTransient<ReportesView>();
                    services.AddTransient<UsuariosView>();
                    services.AddTransient<UsuarioEditorView>();
                    services.AddTransient<UsuarioRolesView>();

                    // Servicios de transacción
                    services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Obtener el DialogService del contenedor de DI en lugar de crear uno nuevo
            var dialogService = _host.Services.GetRequiredService<DialogService>();

            // Registrar las vistas
            dialogService.Register<TipoEquipoEditorViewModel, Views.TipoEquipoEditorView>();
            dialogService.Register<UnidadEditorViewModel, Views.UnidadEditorView>();
            dialogService.Register<EstadoEditorViewModel, Views.EstadoEditorView>();
            dialogService.Register<SedeEditorViewModel, Views.SedeEditorView>();
            dialogService.Register<AreaEditorViewModel, Views.AreaEditorView>();
            dialogService.Register<ZonaEditorViewModel, Views.ZonaEditorView>();
            dialogService.Register<EquipoComputoEditorViewModel, Views.EquipoComputoEditorView>();
            dialogService.Register<HistorialEquipoViewModel, Views.HistorialEquipoView>();
            dialogService.Register<UsuarioEditorViewModel, Views.UsuarioEditorView>();
            dialogService.Register<UsuarioRolesViewModel, Views.UsuarioRolesView>();

            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

                try
                {
                    var resetOnStartup = config.GetValue<bool>("Database:ResetOnStartup");
                    if (resetOnStartup)
                    {
                        await db.Database.EnsureDeletedAsync();
                    }
                    await db.Database.MigrateAsync();
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    await authService.CrearUsuarioAdministradorSiNoExisteAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al preparar la base de datos: {ex.Message}",
                        "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
            }

            // Evitar que la app cierre al cerrar el Login
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Mostrar Login como diálogo modal
            var loginVM = _host.Services.GetRequiredService<LoginViewModel>();
            var loginView = new LoginView { DataContext = loginVM };
            var resultado = loginView.ShowDialog();

            if (loginVM.LoginExitoso)
            {
                try
                {
                    var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                    Current.MainWindow = mainWindow;
                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    mainWindow.Show();

                    // SUSCRIPCIÓN CENTRALIZADA AL LOGOUT
                    var session = _host.Services.GetRequiredService<ISessionService>();
                    session.SesionCambiada += (s, loggedIn) =>
                    {
                        if (!loggedIn)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                // Cambiar el ShutdownMode antes de cerrar la ventana principal
                                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                                // Cerrar ventana principal actual de forma segura
                                var main = Current.MainWindow;
                                if (main != null)
                                {
                                    // Hide es opcional pero evita parpadeos
                                    main.Hide();
                                    main.Close();
                                }

                                // Mostrar login nuevamente
                                var vm = _host.Services.GetRequiredService<LoginViewModel>();
                                var view = new LoginView { DataContext = vm };
                                var ok = view.ShowDialog();

                                if (vm.LoginExitoso)
                                {
                                    var newMain = _host.Services.GetRequiredService<MainWindow>();
                                    Current.MainWindow = newMain;
                                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                                    newMain.Show();
                                }
                                else
                                {
                                    // No reabrir: cerrar aplicación
                                    Current.Shutdown();
                                }
                            });
                        }
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al mostrar la ventana principal: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                }
            }
            else
            {
                Shutdown();
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}