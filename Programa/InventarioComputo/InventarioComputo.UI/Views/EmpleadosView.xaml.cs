using System.Windows;
using System.Windows.Controls;

namespace InventarioComputo.UI.Views
{
    public partial class EmpleadosView : UserControl
    {
        public EmpleadosView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.EmpleadosViewModel viewModel)
            {
                viewModel.LoadedCommand.Execute(null);
            }
        }
    }
}