using Desktop.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Desktop
{
    public partial class Main : Window, INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<TaskItem> _taskList;
        private ObservableCollection<TaskItem> _filteredTaskList;
        private TaskItem _selectedTask;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TaskItem> TaskList
        {
            get => _taskList;
            set { _taskList = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TaskItem> FilteredTaskList
        {
            get => _filteredTaskList;
            set { _filteredTaskList = value; OnPropertyChanged(); }
        }

        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        public Main()
        {
            InitializeComponent();
            DataContext = this;

            TaskList = new ObservableCollection<TaskItem>
            {
                new TaskItem("Go fishing with Stephen", new DateTime(2022, 1, 1, 9, 0, 0), "Отдых"),
                new TaskItem("Go fishing with Stephen", new DateTime(2022, 1, 1, 9, 0, 0), "Отдых"),
                new TaskItem("Read the book Zlatan", new DateTime(2022, 1, 1, 9, 0, 0), "Учеба"),
                new TaskItem("Meet according with design team...", new DateTime(2022, 1, 1, 9, 0, 0), "Работа"),
                new TaskItem("Meet according with design team...", new DateTime(2022, 1, 1, 9, 0, 0), "Работа"),
                new TaskItem("Meet according with design team...", new DateTime(2022, 1, 1, 9, 0, 0), "Работа"),
            };

            FilteredTaskList = new ObservableCollection<TaskItem>(TaskList);

            if (UserRepository.CurrentUser != null)
            {
                Username = UserRepository.CurrentUser.Username;
            }
            else
            {
                Username = "Username";
            }
        }

        private void ShowAllTasks_Click(object sender, RoutedEventArgs e)
        {
            FilteredTaskList = new ObservableCollection<TaskItem>(TaskList);
        }

        private void FilterByCategory_Click(object sender, RoutedEventArgs e)
        {
            string category = (sender as System.Windows.Controls.Label)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(category))
            {
                FilteredTaskList = new ObservableCollection<TaskItem>(TaskList.Where(task => task.Category == category));
            }
        }

        private void CompleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" выполнена!");
                TaskList.Remove(SelectedTask);
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" удалена!");
                TaskList.Remove(SelectedTask);
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TaskItem : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _date;
        private string _category;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        public string FormattedDate => Date.ToString("f", new CultureInfo("ru-RU"));

        public TaskItem(string name, DateTime date, string category)
        {
            Name = name;
            Date = date;
            Category = category;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NullToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}