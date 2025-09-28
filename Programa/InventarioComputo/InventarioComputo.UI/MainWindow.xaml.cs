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
        private const double MENU_COLLAPSED_WIDTH = 50;
        private const double ANIMATION_DURATION = 0.3; // en segundos

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                // Destino de la animación
                double targetWidth = toggleButton.IsChecked == true ? MENU_EXPANDED_WIDTH : MENU_COLLAPSED_WIDTH;

                // Crear animación
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = MenuPanel.ActualWidth,
                    To = targetWidth,
                    Duration = TimeSpan.FromSeconds(ANIMATION_DURATION),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                // Iniciar animación
                MenuPanel.BeginAnimation(FrameworkElement.WidthProperty, animation);
            }
        }
    }
}