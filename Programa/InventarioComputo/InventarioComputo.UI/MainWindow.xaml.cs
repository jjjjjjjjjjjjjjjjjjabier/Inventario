using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI
{
    public partial class MainWindow : Window
    {
        private const double MENU_EXPANDED_WIDTH = 200;
        private const double MENU_COLLAPSED_WIDTH = 60;
        private const double ANIMATION_DURATION = 0.25; // segundos

        private bool _menuExpanded = true;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Aseguramos que el menú comience expandido
            if (MenuPanel != null)
            {
                MenuPanel.Width = MENU_EXPANDED_WIDTH;
                if (MenuToggleButton != null)
                    MenuToggleButton.IsChecked = true;
            }

            // Suscribirse al evento SizeChanged de la ventana para manejar el redimensionamiento
            SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Si la ventana se hace muy pequeña, colapsamos el menú automáticamente
            if (e.NewSize.Width < 800 && _menuExpanded)
            {
                _menuExpanded = false;
                MenuToggleButton.IsChecked = false;
                AnimateMenuWidth(MENU_COLLAPSED_WIDTH);
            }
        }

        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            _menuExpanded = true;
            AnimateMenuWidth(MENU_EXPANDED_WIDTH);
        }

        private void MenuToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _menuExpanded = false;
            AnimateMenuWidth(MENU_COLLAPSED_WIDTH);
        }

        private void AnimateMenuWidth(double targetWidth)
        {
            if (MenuPanel == null) return;

            var animation = new DoubleAnimation
            {
                From = MenuPanel.ActualWidth,
                To = targetWidth,
                Duration = TimeSpan.FromSeconds(ANIMATION_DURATION),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            MenuPanel.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }

        // Respaldo: si el template del ToggleButton no dispara Checked/Unchecked
        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            _menuExpanded = !_menuExpanded;
            AnimateMenuWidth(_menuExpanded ? MENU_EXPANDED_WIDTH : MENU_COLLAPSED_WIDTH);

            if (sender is ToggleButton tb)
                tb.IsChecked = _menuExpanded;
        }
    }
}