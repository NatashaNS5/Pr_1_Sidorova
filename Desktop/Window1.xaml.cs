using Desktop.Repository;
using Desktop.Utiles;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Todo.Entities;

namespace Desktop
{
    public partial class Window1 : Window
    {
        private UserRepository userRepository = new UserRepository();

        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
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

            string errorMessage;
            bool isRegistered = UserRepository.RegisterUser(username, email, password, out errorMessage);

            if (!isRegistered)
            {
                MessageBox.Show(errorMessage);
            }
            else
            {
                MessageBox.Show("Регистрация успешна!");
            }
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
            MessageBox.Show("Вернуться назад");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TransitionToMainWindow();
        }
        private async void TransitionToMainWindow()
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

                var MainWindow = new MainWindow();
                MainWindow.Opacity = 0;
                MainWindow.Show();

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                MainWindow.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                this.Close();
            };
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(500);
        }
    }
}