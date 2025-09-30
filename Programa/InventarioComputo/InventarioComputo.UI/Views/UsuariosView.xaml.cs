using System.Windows;
using System.Windows.Controls;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI.Views
{
    public partial class UsuariosView : UserControl
    {
        public UsuariosView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UsuariosViewModel vm && vm.LoadedCommand != null)
                vm.LoadedCommand.Execute(null);
        }
    }
}