using System;
using System.Windows;
using System.Windows.Controls;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI.Views
{
    public partial class TiposEquipoView : UserControl
    {
        public TiposEquipoView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TiposEquipoViewModel viewModel)
            {
                viewModel.LoadedCommand.Execute(null);
            }
        }
    }
}