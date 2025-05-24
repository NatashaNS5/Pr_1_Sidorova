using Desktop.Repository;
using Desktop.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Desktop.data.Repository;

namespace Desktop.View
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private readonly AuthRepository _authRepository;
        public Page1()
        {
            InitializeComponent();
            _authRepository = new AuthRepository();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string username = textBox.Text;
            string email = textBox1.Text;
            string password = textBox2.Text;
            string confirmPassword = textBox3.Text;

            if (!username.ValidateUsername())
            {
                MessageBox.Show("Имя пользователя должно быть не менее 3 символов.");
                return;
            }
            if (!email.ValidateEmail())
            {
                MessageBox.Show("Введите корректную почту.");
                return;
            }
            if (!password.ValidatePassword())
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов.");
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            if (!await IsServerAvailable())
            {
                MessageBox.Show("Сервер недоступен. Пожалуйста, попробуйте позже.");
                return;
            }

            var (success, errorMessage) = await _authRepository.RegisterUserAsync(username, email, password);

            if (!success)
            {
                MessageBox.Show(errorMessage);
            }
            else
            {
                MessageBox.Show("Регистрация успешна!");
                TransitionToMainWindow();
            }
        }


        private async Task<bool> IsServerAvailable()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://45.144.64.179/api/health");
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            watermark.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            watermark1.Visibility = string.IsNullOrEmpty(textBox1.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            watermark2.Visibility = string.IsNullOrEmpty(textBox2.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            watermark3.Visibility = string.IsNullOrEmpty(textBox3.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вернуться назад");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TransitionToMainWindow();
        }

        private void TransitionToMainWindow()
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.5),
                FillBehavior = FillBehavior.Stop
            };

            fadeOutAnimation.Completed += (s, e) =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();

                Window.GetWindow(this)?.Close();
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }
    }
}