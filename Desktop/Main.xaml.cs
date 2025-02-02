using Desktop.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Desktop
{
    public partial class Main : Window, INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<TaskItem> _filteredTaskList;
        private TaskItem _selectedTask;
        private TaskRepository _taskRepository;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
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

        public TaskRepository TaskRepository
        {
            get => _taskRepository;
            set => _taskRepository = value;
        }

        public Main()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += (s, e) => OpenWithFadeIn();
            _taskRepository = new TaskRepository();
            FilteredTaskList = new ObservableCollection<TaskItem>();

            if (UserRepository.CurrentUser != null)
            {
                Username = UserRepository.CurrentUser.Username;
            }
            else
            {
                Username = "Username";
            }
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

        private void CompleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                _taskRepository.CompleteTask(SelectedTask);
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" выполнена!");
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
            }
        }

        private void ShowAllTasks_Click(object sender, RoutedEventArgs e)
        {
            FilteredTaskList = _taskRepository.GetAllTasks();
        }

        private void OpenHistoryWindow_Click(object sender, RoutedEventArgs e)
        {
            CloseWithFadeOut();
            Window2 historyWindow = new Window2(_taskRepository.CompletedTasks);
            historyWindow.Show();
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

        private void FilterByCategory_Click(object sender, RoutedEventArgs e)
        {
            string category = (sender as System.Windows.Controls.Label)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(category))
            {
                FilteredTaskList = _taskRepository.GetTasksByCategory(category);
            }
        }
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" удалена!");
                _taskRepository.RemoveTask(SelectedTask);
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow();
            AnimateWindow(addTaskWindow);

            if (addTaskWindow.ShowDialog() == true)
            {
                var newTask = addTaskWindow.NewTask;
                if (newTask != null)
                {
                    _taskRepository.AddTask(newTask);
                    FilteredTaskList.Add(newTask);
                }
            }
        }

        private void AnimateWindow(Window window)
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            window.Opacity = 0;
            window.BeginAnimation(Window.OpacityProperty, animation);
        }
    }

    public class TaskItem : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _date;
        private string _category;
        private string _description;

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

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string FormattedDate => Date.ToString("f", new CultureInfo("ru-RU"));

        public TaskItem(string name, DateTime date, string category, string description = "")
        {
            Name = name;
            Date = date;
            Category = category;
            Description = description;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}