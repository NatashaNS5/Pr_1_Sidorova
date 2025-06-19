using Desktop.Utiles;
using Desktop.View;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        public TaskItem NewTask { get; private set; }

        public AddTaskWindow()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                DatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(CategoryTextBox.Text))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var date = DatePicker.SelectedDate.Value;
                if (!TimeSpan.TryParse(TimeTextBox.Text, out var time))
                {
                    MessageBox.Show("Введите время в формате ЧЧ:ММ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var taskDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
                var category = CategoryTextBox.Text.Trim();

                NewTask = new TaskItem(NameTextBox.Text, taskDateTime, category)
                {
                    Description = DescriptionTextBox.Text
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithFadeOut();
        }

        private async void CloseWithFadeOut()
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.5),
                FillBehavior = FillBehavior.Stop
            };

            fadeOutAnimation.Completed += (s, e) => Close();
            BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(500);
        }
    }
}