using System.Windows;
using System.Windows.Controls;

namespace InventarioComputo.UI.Views
{
    public partial class UnidadesView : UserControl
    {
        public UnidadesView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Desencadenar comando de carga al iniciar la vista
            if (DataContext is ViewModels.UnidadesViewModel vm)
            {
                vm.LoadedCommand.Execute(null);
            }
        }
    }
}