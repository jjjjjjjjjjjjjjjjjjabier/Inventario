using System.Windows;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI.Views
{
    public partial class HistorialEquipoView : Window
    {
        public HistorialEquipoView()
        {
            InitializeComponent();
        }

        // Este m�todo permite inicializar la vista con el ID del equipo
        public void SetViewModel(HistorialEquipoViewModel viewModel, int equipoId)
        {
            DataContext = viewModel;

            // Asegurarse de que la ventana est� cargada antes de cargar datos
            Loaded += async (s, e) =>
            {
                await viewModel.CargarHistorialAsync(equipoId);
            };
        }
    }
}