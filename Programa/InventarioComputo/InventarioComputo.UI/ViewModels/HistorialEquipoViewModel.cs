using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InventarioComputo.UI.ViewModels
{
    public partial class HistorialEquipoViewModel : BaseViewModel
    {
        private readonly IMovimientoService _movimientoSrv;
        private readonly IEquipoComputoService _equipoSrv;

        [ObservableProperty]
        private EquipoComputo? _equipo;

        // NUEVO: controla el overlay “No hay historial”
        [ObservableProperty]
        private bool _hasNoMovimientos = true;

        public string Titulo { get; private set; } = "Historial de Movimientos";

        // CAMBIO: ahora es una colección de VM de ítem
        public ObservableCollection<MovimientoItemViewModel> Movimientos { get; } = new();

        public HistorialEquipoViewModel(
            IMovimientoService movimientoSrv,
            IEquipoComputoService equipoSrv,
            ILogger<HistorialEquipoViewModel> logger)
        {
            _movimientoSrv = movimientoSrv;
            _equipoSrv = equipoSrv;
            Logger = logger;
        }

        public async Task CargarHistorialAsync(int equipoId)
        {
            IsBusy = true;
            try
            {
                Movimientos.Clear();

                var equipo = await _equipoSrv.ObtenerPorIdAsync(equipoId);
                Equipo = equipo;
                if (Equipo != null)
                {
                    Titulo = $"Historial de Movimientos - {Equipo.NumeroSerie}";
                    OnPropertyChanged(nameof(Titulo));

                    var historial = await _movimientoSrv.ObtenerHistorialPorEquipoAsync(equipoId);

                    foreach (var mov in historial)
                        Movimientos.Add(new MovimientoItemViewModel(mov));

                    HasNoMovimientos = Movimientos.Count == 0;
                }
                else
                {
                    HasNoMovimientos = true;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error al cargar historial para equipo {EquipoId}", equipoId);
                HasNoMovimientos = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Close()
        {
            var window = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
}