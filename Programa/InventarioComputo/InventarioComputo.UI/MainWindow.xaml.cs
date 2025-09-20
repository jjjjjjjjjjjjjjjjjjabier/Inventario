using InventarioComputo.UI.ViewModels;
using System.Windows;

namespace InventarioComputo.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}