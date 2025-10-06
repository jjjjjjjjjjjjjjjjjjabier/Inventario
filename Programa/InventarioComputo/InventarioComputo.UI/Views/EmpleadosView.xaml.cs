using System.Windows;
using System.Windows.Controls;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI.Views
{
    public partial class EmpleadosView : UserControl
    {
        private bool _loadedOnce;

        public EmpleadosView()
        {
            InitializeComponent();
            Loaded += EmpleadosView_Loaded;
        }

        private async void EmpleadosView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loadedOnce) return;
            _loadedOnce = true;

            if (DataContext is EmpleadosViewModel vm)
                await vm.LoadedAsync();
        }
    }
}