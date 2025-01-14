using Desktop.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    public partial class Window2 : Window, INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<TaskItem> _filteredTaskList;
        private TaskItem _selectedTask;

        public ObservableCollection<TaskItem> CompletedTasks { get; }

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public ObservableCollection<TaskItem> FilteredTaskList
        {
            get => _filteredTaskList;
            set
            {
                if (_filteredTaskList != value)
                {
                    _filteredTaskList = value;
                    OnPropertyChanged(nameof(FilteredTaskList));
                }
            }
        }

        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;
                    OnPropertyChanged(nameof(SelectedTask));
                }
            }
        }

        public Window2(ObservableCollection<TaskItem> completedTasks)
        {
            InitializeComponent();
            CompletedTasks = completedTasks ?? throw new ArgumentNullException(nameof(completedTasks));
            FilteredTaskList = new ObservableCollection<TaskItem>(CompletedTasks);
            DataContext = this;

            Username = UserRepository.CurrentUser?.Username ?? "Default Username";
        }

        private void FilterByCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Label label && label.Tag is string category && !string.IsNullOrEmpty(category))
            {
                FilteredTaskList = new ObservableCollection<TaskItem>(
                    CompletedTasks.Where(task => task.Category == category)
                );
                SelectedTask = null;
            }
        }

        private void ShowAllTasks_Click(object sender, RoutedEventArgs e)
        {
            FilteredTaskList = new ObservableCollection<TaskItem>(CompletedTasks);
            SelectedTask = null;
        }

        private void OnTaskSelected(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                SelectedTask = listBox.SelectedItem as TaskItem;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Чтобы добавить новую задачу, нужно выйти из истории.");
        }
    }
}