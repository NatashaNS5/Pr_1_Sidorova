using Desktop.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Desktop.View
{
    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page, INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<TaskItem> _filteredTaskList;
        private TaskItem _selectedTask;
        private TaskRepository _taskRepository;
        private bool _isShowingCompletedTasks;

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

        public bool IsShowingCompletedTasks
        {
            get => _isShowingCompletedTasks;
            set { _isShowingCompletedTasks = value; OnPropertyChanged(); }
        }

        public Page2()
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

        private void CreateFirstTaskButton_Click(object sender, RoutedEventArgs e)
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
                    ShowMainContent();
                }
            }
        }

        private async void ShowMainContent()
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            var scaleOutAnimationX = new DoubleAnimation
            {
                From = 1.0,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            var scaleOutAnimationY = new DoubleAnimation
            {
                From = 1.0,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            EmptyScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleOutAnimationX);
            EmptyScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleOutAnimationY);
            EmptyGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            fadeOutAnimation.Completed += (s, args) =>
            {
                EmptyGrid.Visibility = Visibility.Collapsed;
                MainGrid.Visibility = Visibility.Visible;

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                var scaleInAnimationX = new DoubleAnimation
                {
                    From = 0.8,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                var scaleInAnimationY = new DoubleAnimation
                {
                    From = 0.8,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                MainScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleInAnimationX);
                MainScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleInAnimationY);
                MainGrid.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                ShowAllTasks_Click(null, null);
            };

            EmptyGrid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            await Task.Delay(500);
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
            window.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void CompleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = true;
                _taskRepository.CompleteTask(SelectedTask);
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" выполнена!");
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
            }
        }

        private void ShowAllTasks_Click(object sender, RoutedEventArgs e)
        {
            IsShowingCompletedTasks = false;
            FilteredTaskList = _taskRepository.GetAllTasks();
        }

        private void ShowCompletedTasks_Click(object sender, RoutedEventArgs e)
        {
            IsShowingCompletedTasks = true;
            FilteredTaskList = _taskRepository.CompletedTasks;
        }

        private void FilterByCategory_Click(object sender, RoutedEventArgs e)
        {
            string category = (sender as Label)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(category))
            {
                if (IsShowingCompletedTasks)
                {
                    FilteredTaskList = new ObservableCollection<TaskItem>(
                        _taskRepository.CompletedTasks.Where(task => task.Category == category)
                    );
                }
                else
                {
                    FilteredTaskList = _taskRepository.GetTasksByCategory(category);
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TaskItem : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _date;
        private string _category;
        private string _description;
        private bool _isCompleted;

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

        public bool IsCompleted
        {
            get => _isCompleted;
            set { _isCompleted = value; OnPropertyChanged(); }
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

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return TextDecorations.Strikethrough;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
