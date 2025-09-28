using System.Windows;
using System.Windows.Controls;

namespace InventarioComputo.UI.Behaviors
{
    public static class PasswordBoxBehavior
    {
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(PasswordBoxBehavior),
                new PropertyMetadata(false, OnAttachChanged));

        public static bool GetAttach(DependencyObject obj) => (bool)obj.GetValue(AttachProperty);
        public static void SetAttach(DependencyObject obj, bool value) => obj.SetValue(AttachProperty, value);

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBoxBehavior),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));

        public static string GetPassword(DependencyObject obj) => (string)obj.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject obj, string value) => obj.SetValue(PasswordProperty, value);

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxBehavior));

        private static bool GetIsUpdating(DependencyObject obj) => (bool)obj.GetValue(IsUpdatingProperty);
        private static void SetIsUpdating(DependencyObject obj, bool value) => obj.SetValue(IsUpdatingProperty, value);

        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox box)
            {
                if ((bool)e.NewValue)
                {
                    box.PasswordChanged += PasswordChanged;
                }
                else
                {
                    box.PasswordChanged -= PasswordChanged;
                }
            }
        }

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox box)
            {
                box.PasswordChanged -= PasswordChanged;

                if (!GetIsUpdating(box))
                {
                    box.Password = e.NewValue?.ToString() ?? string.Empty;
                }

                box.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                SetIsUpdating(box, true);
                SetPassword(box, box.Password);
                SetIsUpdating(box, false);
            }
        }
    }
}