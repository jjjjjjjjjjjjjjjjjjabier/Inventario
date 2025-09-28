using System.Windows;
using System.Windows.Controls;
using InventarioComputo.UI.ViewModels;

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
            if (DataContext is UnidadesViewModel viewModel)
            {
                viewModel.LoadedCommand.Execute(null);
            }
        }
    }
}