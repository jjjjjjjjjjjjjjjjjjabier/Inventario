using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Infrastructure.Persistencia;
using InventarioComputo.Infrastructure.Repositories;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels;
using InventarioComputo.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;

namespace InventarioComputo.UI
{
    // **CORRECCIÓN:** Se especifica la ruta completa a System.Windows.Application
    // para resolver la ambigüedad con el namespace InventarioComputo.Application.
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
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<InventarioDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Repositorios
                    services.AddScoped<ISedeRepository, SedeRepository>();
                    services.AddScoped<IAreaRepository, AreaRepository>();
                    services.AddScoped<IZonaRepository, ZonaRepository>();
                    services.AddScoped<IEstadoRepository, EstadoRepository>();
                    services.AddScoped<IUnidadRepository, UnidadRepository>();
                    services.AddScoped<ITipoEquipoRepository, TipoEquipoRepository>();
                    services.AddScoped<IEquipoComputoRepository, EquipoComputoRepository>();

                    // Servicios de Aplicación
                    services.AddScoped<ISedeService, SedeService>();
                    services.AddScoped<IAreaService, AreaService>();
                    services.AddScoped<IZonaService, ZonaService>();
                    services.AddScoped<IEstadoService, EstadoService>();
                    services.AddScoped<IUnidadService, UnidadService>();
                    services.AddScoped<ITipoEquipoService, TipoEquipoService>();
                    services.AddScoped<IEquipoComputoService, EquipoComputoService>();

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

                    // Vistas (Ventanas)
                    services.AddTransient<EstadoEditorView>();
                    services.AddTransient<UnidadEditorView>();
                    services.AddTransient<TipoEquipoEditorView>();
                    services.AddTransient<SedeEditorView>();
                    services.AddTransient<AreaEditorView>();
                    services.AddTransient<ZonaEditorView>();
                    services.AddTransient<EquipoComputoEditorView>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var dialogService = _host.Services.GetRequiredService<DialogService>();
            dialogService.Register<EstadoEditorViewModel, EstadoEditorView>();
            dialogService.Register<UnidadEditorViewModel, UnidadEditorView>();
            dialogService.Register<TipoEquipoEditorViewModel, TipoEquipoEditorView>();
            dialogService.Register<SedeEditorViewModel, SedeEditorView>();
            dialogService.Register<AreaEditorViewModel, AreaEditorView>();
            dialogService.Register<ZonaEditorViewModel, ZonaEditorView>();
            dialogService.Register<EquipoComputoEditorViewModel, EquipoComputoEditorView>();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

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