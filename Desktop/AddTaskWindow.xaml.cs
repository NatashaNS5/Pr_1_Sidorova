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
                CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var date = DatePicker.SelectedDate.Value;

                if (!TimeSpan.TryParse(TimeTextBox.Text, out var time))
                {
                    MessageBox.Show("Введите корректное время в формате HH:mm.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var taskDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);

                var selectedCategory = (CategoryComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                NewTask = new TaskItem(NameTextBox.Text, taskDateTime, selectedCategory)
                {
                    Description = DescriptionTextBox.Text
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            fadeOutAnimation.Completed += (s, e) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(500);
        }
    }
}