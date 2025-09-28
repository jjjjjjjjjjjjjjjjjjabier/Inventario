using System.Windows;
using System.Windows.Input;

namespace InventarioComputo.UI.Behaviors
{
    public static class LoadedCommandBehavior
    {
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached(
                "LoadedCommand",
                typeof(ICommand),
                typeof(LoadedCommandBehavior),
                new PropertyMetadata(null, OnLoadedCommandChanged));

        public static ICommand GetLoadedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadedCommandProperty);
        }

        public static void SetLoadedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedCommandProperty, value);
        }

        private static void OnLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (e.OldValue != null)
                {
                    element.Loaded -= Element_Loaded;
                }

                if (e.NewValue != null)
                {
                    element.Loaded += Element_Loaded;
                }
            }
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var command = GetLoadedCommand(element);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }
    }
}