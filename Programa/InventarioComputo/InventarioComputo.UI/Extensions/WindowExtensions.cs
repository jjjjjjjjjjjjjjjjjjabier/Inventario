using System.Linq;
using System.Windows;

namespace InventarioComputo.UI.Extensions
{
    public static class WindowExtensions
    {
        public static void CloseWindowOfViewModel(this object viewModel)
        {
            var window = System.Windows.Application.Current.Windows.OfType<Window>()
                .SingleOrDefault(w => w.DataContext == viewModel);
            window?.Close();
        }
    }
}