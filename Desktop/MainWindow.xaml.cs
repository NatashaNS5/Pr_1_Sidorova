using Desktop.Repository;
using Desktop.Utiles;
using Desktop.View;
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
using static Desktop.data.Repository;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private readonly AuthRepository _authRepository;
        public MainWindow()
        {
            InitializeComponent();
            _authRepository = new AuthRepository();
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

            var transform = new ScaleTransform();
            button.RenderTransform = transform;
            button.RenderTransformOrigin = new Point(0.5, 0.5);

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TransitionToPage1();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = textBox.Text;
                string password = textBox1.Text;

                var (success, errorMessage) = await _authRepository.AuthenticateUserAsync(email, password);

                if (!success)
                {
                    MessageBox.Show(errorMessage);
                }
                else
                {
                    MessageBox.Show("Вход успешно выполнен!");
                    TransitionToPage2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermark();
        }

        private void UpdateWatermark()
        {
            watermark.Visibility = string.IsNullOrWhiteSpace(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBox_TextChanged1(object sender, TextChangedEventArgs e)
        {
            UpdateWatermark1();
        }

        private void UpdateWatermark1()
        {
            watermark1.Visibility = string.IsNullOrWhiteSpace(textBox1.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TransitionToPage1()
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
                var mainFrame = this.FindName("MainFrame") as Frame;
                if (mainFrame != null)
                {
                    mainFrame.Navigate(new Page1());
                }
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private void TransitionToPage2()
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
                var mainFrame = this.FindName("MainFrame") as Frame;
                if (mainFrame != null)
                {
                    mainFrame.Navigate(new Page2());
                }
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }
    }
}
