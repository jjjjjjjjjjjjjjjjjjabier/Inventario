using CommunityToolkit.Mvvm.ComponentModel;

namespace InventarioComputo.UI.ViewModels
{
    public sealed partial class RolCheckboxViewModel : ObservableObject
    {
        public int RolId { get; }
        public string Nombre { get; }

        [ObservableProperty]
        private bool _isSelected;

        public RolCheckboxViewModel(int rolId, string nombre, bool isSelected = false)
        {
            RolId = rolId;
            Nombre = nombre;
            _isSelected = isSelected;
        }
    }
}