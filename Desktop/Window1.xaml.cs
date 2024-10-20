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
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
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

            public static bool ValidateUsername(string username)
            {
                // Имя должно быть не менее 3 символов
                return username.Length >= 3;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string username = textBox.Text; // Имя пользователя
            string email = textBox1.Text; // Email
            string password = textBox2.Text; // Пароль
            string confirmPassword = textBox3.Text; // Подтверждение пароля

            // Валидация данных
            if (!InputValidator.ValidateUsername(username))
            {
                MessageBox.Show("Имя пользователя должно быть не менее 3 символов.");
                return;
            }

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

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            // Данные корректны
            MessageBox.Show("Регистрация успешна!");
            Main_empty main_empty = new Main_empty();

            main_empty.Show();

            this.Close();
        }
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            watermark.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            watermark1.Visibility = string.IsNullOrEmpty(textBox1.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBox2_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            watermark2.Visibility = string.IsNullOrEmpty(textBox2.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBox3_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            watermark3.Visibility = string.IsNullOrEmpty(textBox3.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для кнопки "Назад"
            MessageBox.Show("Вернуться назад");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }
    }
}