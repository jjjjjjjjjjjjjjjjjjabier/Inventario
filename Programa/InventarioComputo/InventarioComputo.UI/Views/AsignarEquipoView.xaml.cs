using System.Windows;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI.Views
{
    public partial class AsignarEquipoView : Window
    {
        public AsignarEquipoView()
        {
            InitializeComponent();
        }

        // Este método nos permite inicializar el ViewModel con los datos del equipo
        public void SetViewModel(AsignarEquipoViewModel viewModel)
        {
            DataContext = viewModel;
            if (viewModel != null)
            {
                viewModel.RequestClose += (s, e) => Close();
            }
        }
    }
}