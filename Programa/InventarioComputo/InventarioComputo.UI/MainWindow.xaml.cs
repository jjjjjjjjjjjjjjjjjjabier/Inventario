using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using InventarioComputo.UI.ViewModels;

namespace InventarioComputo.UI
{
    public partial class MainWindow : Window
    {
        private const double MENU_EXPANDED_WIDTH = 200;
        private const double MENU_COLLAPSED_WIDTH = 50;
        private const double ANIMATION_DURATION = 0.2; // segundos

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private System.Windows.Controls.Panel? GetMenuPanel() => FindName("MenuPanel") as System.Windows.Controls.Panel;

        private void ExpandirMenu(bool expandir)
        {
            var panel = GetMenuPanel();
            if (panel == null) return;
            panel.Visibility = expandir ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
            => AnimateMenuWidth(MENU_EXPANDED_WIDTH);

        private void MenuToggleButton_Unchecked(object sender, RoutedEventArgs e)
            => AnimateMenuWidth(MENU_COLLAPSED_WIDTH);

        private void AnimateMenuWidth(double targetWidth)
        {
            var animation = new DoubleAnimation
            {
                From = MenuPanel.ActualWidth,
                To = targetWidth,
                Duration = TimeSpan.FromSeconds(ANIMATION_DURATION),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            MenuPanel.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }
    }
}