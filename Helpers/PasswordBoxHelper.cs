using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace car_storage_odometer.Helpers 
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
    DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnBoundPasswordChanged 
        ));

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox == null)
                return;
           
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            
            if (!string.Equals((string)e.NewValue, passwordBox.Password))
                passwordBox.Password = (string)e.NewValue;

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            SetBoundPassword(passwordBox, passwordBox.Password);

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, Attach));

        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if ((bool)e.NewValue)
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        public static string HashPassword(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}