using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DromHubSettings.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject d)
        {
            return (string)d.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject d, string value)
        {
            d.SetValue(BoundPasswordProperty, value);
        }

        private static bool _updatingPassword;

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                if (!_updatingPassword)
                {
                    passwordBox.Password = e.NewValue as string ?? string.Empty;
                }
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _updatingPassword = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _updatingPassword = false;
            }
        }
    }
}
