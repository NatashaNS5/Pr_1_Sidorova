using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static class InputValidator
        {
            // Исправлено: правильное регулярное выражение для проверки email
            private static readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            public static bool ValidateEmail(string email)
            {
                return Regex.IsMatch(email, emailPattern);
            }

            public static bool ValidatePassword(string password)
            {
                // Пароль должен быть не менее 6 символов
                return password.Length >= 6;
            }
        }

            private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();

            window1.Show();

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = textBox.Text; // Email
            string password = textBox1.Text; // Пароль

            if (!InputValidator.ValidateEmail(email))
            {
                MessageBox.Show("Введите корректную почту.");
                return;
            }

            if (!InputValidator.ValidatePassword(password))
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов.");
                return;
            }
            // Данные корректны
            MessageBox.Show("Вход успешно выполнен!");

            Main_empty main_empty = new Main_empty();

            main_empty.Show();

            this.Close();
        }
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateWatermark();
        }

        private void UpdateWatermark()
        {
            watermark.Visibility = string.IsNullOrWhiteSpace(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void TextBox_TextChanged1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateWatermark1();
        }
        private void UpdateWatermark1()
        {
            watermark1.Visibility = string.IsNullOrWhiteSpace(textBox1.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

