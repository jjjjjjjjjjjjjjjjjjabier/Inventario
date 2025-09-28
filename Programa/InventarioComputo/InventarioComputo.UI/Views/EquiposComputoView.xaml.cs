using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI.Views
{
    /// <summary>
    /// Lógica de interacción para EquiposComputoView.xaml
    /// </summary>
    public partial class EquiposComputoView : UserControl
    {
        public EquiposComputoView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EquiposComputoViewModel viewModel)
            {
                viewModel.LoadedCommand.Execute(null);
            }
        }
    }
}
