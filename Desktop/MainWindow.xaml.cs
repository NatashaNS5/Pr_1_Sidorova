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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OpenWithFadeIn();
            InitializeButtonAnimations();
        }

        private void OpenWithFadeIn()
        {
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
        private void InitializeButtonAnimations()
        {
            Button button1 = this.FindName("button1") as Button;
            if (button1 != null)
            {
                button1.MouseEnter += (s, e) => AnimateButton(button1, 1.1);
                button1.MouseLeave += (s, e) => AnimateButton(button1, 1.0);
            }

            Button button2 = this.FindName("button2") as Button;
            if (button2 != null)
            {
                button2.MouseEnter += (s, e) => AnimateButton(button2, 1.1);
                button2.MouseLeave += (s, e) => AnimateButton(button2, 1.0);
            }
        }

        private void AnimateButton(Button button, double scale)
        {
            var scaleAnimation = new DoubleAnimation
            {
                To = scale,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var transform = new System.Windows.Media.ScaleTransform();
            button.RenderTransform = transform;
            button.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            transform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleAnimation);
            transform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleAnimation);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TransitionToWindow1();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = textBox.Text;
            string password = textBox1.Text;

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

            string errorMessage;
            bool isRegistered = UserRepository.AuthenticateUser(email, password, out errorMessage);

            if (!isRegistered)
            {
                MessageBox.Show(errorMessage);
            }
            else
            {
                MessageBox.Show("Вход успешно выполнен!");
                TransitionToMain_empty();
            }
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

        private async void TransitionToWindow1()
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
                var Window1 = new Window1();
                Window1.Opacity = 0;
                Window1.Show();

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                Window1.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                this.Close();
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(500);
        }

        private async void TransitionToMain_empty()
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
                var Main_empty = new Main_empty();
                Main_empty.Opacity = 0;
                Main_empty.Show();

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                Main_empty.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                this.Close();
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(500);
        }
    }
}