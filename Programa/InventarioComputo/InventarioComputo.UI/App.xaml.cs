using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Infrastructure.Persistencia;
using InventarioComputo.Infrastructure.Repositories;
using InventarioComputo.Infrastructure.Security;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels;
using InventarioComputo.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InventarioComputo.UI
{
    public partial class App : System.Windows.Application
    {
        private readonly IHost _host;

        public static IServiceProvider Services => ((App)Current)._host.Services;

        public App()
        {
            InitializeComponent();

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
                        options
                            .UseSqlServer(connectionString, sql =>
                                sql.MigrationsAssembly(typeof(InventarioDbContext).Assembly.FullName))
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging()
                    );

                    // Seguridad
                    services.AddSingleton<IPasswordHasher, PasswordHasher>();

                    // Repos
                    services.AddScoped<ISedeRepository, SedeRepository>();
                    services.AddScoped<IAreaRepository, AreaRepository>();
                    services.AddScoped<IZonaRepository, ZonaRepository>();
                    services.AddScoped<IEstadoRepository, EstadoRepository>();
                    services.AddScoped<IUnidadRepository, UnidadRepository>();
                    services.AddScoped<ITipoEquipoRepository, TipoEquipoRepository>();
                    services.AddScoped<IEquipoComputoRepository, EquipoComputoRepository>();
                    services.AddScoped<IUsuarioRepository, UsuarioRepository>();
                    services.AddScoped<IRolRepository, RolRepository>();
                    services.AddScoped<IHistorialMovimientoRepository, HistorialMovimientoRepository>();
                    services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

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
                    services.AddScoped<IEmpleadoService, EmpleadoService>();

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
                    services.AddTransient<EmpleadosViewModel>();
                    services.AddTransient<EmpleadoEditorViewModel>();

                    // Views
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
                    services.AddTransient<EmpleadosView>();
                    services.AddTransient<EmpleadoEditorView>();

                    // UoW
                    services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var dialogService = _host.Services.GetRequiredService<DialogService>();

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
            dialogService.Register<AsignarEquipoViewModel, Views.AsignarEquipoView>();
            dialogService.Register<EmpleadoEditorViewModel, Views.EmpleadoEditorView>();

            using (var scope = _host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                try
                {
                    await EnsureDatabaseReadyAsync(db, config);

                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    await authService.CrearUsuarioAdministradorSiNoExisteAsync();

                    var usuarioSrv = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
                    var rolSrv = scope.ServiceProvider.GetRequiredService<IRolService>();

                    var adminUser = config.GetSection("SeedUsers:Admin");
                    var consultaUser = config.GetSection("SeedUsers:Consulta"); // sin espacio

                    async Task EnsureUserAsync(IConfigurationSection section, string rolNombre)
                    {
                        var userName = section.GetValue<string>("UserName");
                        var fullName = section.GetValue<string>("FullName");
                        var password = section.GetValue<string>("Password");

                        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                            return;

                        var user = await usuarioSrv.ObtenerPorNombreUsuarioAsync(userName);
                        if (user == null)
                        {
                            user = new InventarioComputo.Domain.Entities.Usuario
                            {
                                NombreUsuario = userName,
                                NombreCompleto = fullName ?? userName,
                                Activo = true
                            };
                            user = await usuarioSrv.GuardarAsync(user, password);
                        }

                        var rol = await rolSrv.ObtenerPorNombreAsync(rolNombre);
                        if (rol != null)
                        {
                            var rolesUsuario = await usuarioSrv.ObtenerRolesDeUsuarioAsync(user.Id);
                            var yaTiene = rolesUsuario.Any(r => r.Nombre == rolNombre);
                            if (!yaTiene)
                                await usuarioSrv.AsignarRolUsuarioAsync(user.Id, rol.Id);
                        }
                    }

                    await EnsureUserAsync(adminUser, "Administrador");
                    await EnsureUserAsync(consultaUser, "Consulta");
                }
                catch (Exception ex)
                {
                    string BuildExceptionText(Exception e)
                    {
                        var sb = new StringBuilder();
                        int level = 0;
                        while (e != null)
                        {
                            sb.AppendLine($"[{level}] {e.GetType().Name}: {e.Message}");
                            if (e is Microsoft.EntityFrameworkCore.DbUpdateException dbex && dbex.Entries?.Count > 0)
                            {
                                sb.AppendLine($"   Entidades afectadas: {string.Join(", ", dbex.Entries.Select(en => en.Metadata.Name))}");
                            }
                            e = e.InnerException;
                            level++;
                        }
                        return sb.ToString();
                    }

                    MessageBox.Show(
                        $"Error al preparar la base de datos:\n\n{BuildExceptionText(ex)}",
                        "Error de Inicialización",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
            }

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

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

                    var session = _host.Services.GetRequiredService<ISessionService>();
                    session.SesionCambiada += (s, loggedIn) =>
                    {
                        if (!loggedIn)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                                var main = Current.MainWindow;
                                if (main != null)
                                {
                                    main.Hide();
                                    main.Close();
                                }

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

        private static async Task EnsureDatabaseReadyAsync(InventarioDbContext db, IConfiguration config)
        {
            var cn = db.Database.GetConnectionString();
            Log.Information("Conectando a: {ConnectionString}", cn);

            var resetOnStartup = config.GetValue<bool>("Database:ResetOnStartup");
            if (resetOnStartup)
            {
                Log.Warning("Database:ResetOnStartup=true -> Eliminando base de datos antes de migrar.");
                await db.Database.EnsureDeletedAsync();
            }

            // Intentar migrar. Si no hay migraciones o falla, usar EnsureCreated como fallback de desarrollo.
            try
            {
                await db.Database.MigrateAsync();
            }
            catch (Exception exMigrate)
            {
                Log.Warning(exMigrate, "Database.MigrateAsync falló, intentando Database.EnsureCreatedAsync como fallback.");
                await db.Database.EnsureCreatedAsync();
            }

            // Validar existencia de tabla [dbo].[Usuarios]
            if (!await UsuariosTableExistsAsync(db))
            {
                Log.Warning("Tabla [dbo].[Usuarios] no encontrada tras migrar/crear. Reintentando EnsureCreated...");
                await db.Database.EnsureCreatedAsync();

                if (!await UsuariosTableExistsAsync(db))
                {
                    throw new InvalidOperationException("La tabla [dbo].[Usuarios] no existe. Revisa/regenera las migraciones para alinear el modelo con la base de datos.");
                }
            }

            Log.Information("Validación de tabla [dbo].[Usuarios]: OK");
        }

        private static async Task<bool> UsuariosTableExistsAsync(InventarioDbContext db)
        {
            try
            {
                // Si la tabla existe, el SELECT TOP 0 no lanza excepción. Si no existe, lanzará SqlException.
                await db.Database.ExecuteSqlRawAsync("SELECT TOP 0 [Id] FROM [dbo].[Usuarios];");
                return true;
            }
            catch
            {
                return false;
            }
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