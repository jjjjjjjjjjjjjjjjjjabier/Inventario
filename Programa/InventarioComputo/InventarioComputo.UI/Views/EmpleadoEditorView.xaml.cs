using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventarioComputo.UI.Views
{
    public partial class EmpleadoEditorView : Window
    {
        public EmpleadoEditorView()
        {
            InitializeComponent();
        }

        private static readonly Regex Digitos = new(@"^\d+$");

        private void TelefonoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Digitos.IsMatch(e.Text);
        }

        private void TelefonoTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var texto = (string)e.DataObject.GetData(DataFormats.Text);
                if (!texto.All(char.IsDigit))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}