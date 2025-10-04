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

        // Inicializa el ViewModel sin suscribirse a eventos inexistentes
        public void SetViewModel(AsignarEquipoViewModel viewModel)
        {
            DataContext = viewModel;
            // El cierre de la ventana debe ser iniciado por el ViewModel
            // usando this.CloseWindowOfViewModel() (extensión ya presente en el proyecto).
        }
    }
}