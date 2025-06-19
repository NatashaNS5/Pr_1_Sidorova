using Desktop.Utiles;
using Desktop.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Desktop.Repository
{
    public class TaskRepository
    {
        private ObservableCollection<TaskItem> _taskList;
        private ObservableCollection<TaskItem> _completedTasks;
        private Dictionary<string, SolidColorBrush> _categoryColors;

        public ObservableCollection<TaskItem> TaskList
        {
            get => _taskList;
            set => _taskList = value;
        }

        public ObservableCollection<TaskItem> CompletedTasks
        {
            get => _completedTasks;
            set => _completedTasks = value;
        }

        public Dictionary<string, SolidColorBrush> CategoryColors
        {
            get => _categoryColors;
        }

        public TaskRepository()
        {
            _taskList = new ObservableCollection<TaskItem>();
            _completedTasks = new ObservableCollection<TaskItem>();
            _categoryColors = new Dictionary<string, SolidColorBrush>();
        }

        public void AddTask(TaskItem task)
        {
            _taskList.Add(task);
            if (!_categoryColors.ContainsKey(task.Category))
            {
                _categoryColors[task.Category] = GetRandomColor();
            }
        }

        public void RemoveTask(TaskItem task)
        {
            _taskList.Remove(task);
            if (!_taskList.Any(t => t.Category == task.Category) && !_completedTasks.Any(t => t.Category == task.Category))
            {
                _categoryColors.Remove(task.Category);
            }
        }

        public void CompleteTask(TaskItem task)
        {
            if (_taskList.Contains(task))
            {
                _taskList.Remove(task);
                _completedTasks.Add(task);
            }
        }

        public ObservableCollection<TaskItem> GetTasksByCategory(string category)
        {
            return new ObservableCollection<TaskItem>(_taskList.Where(task => task.Category == category));
        }

        public ObservableCollection<TaskItem> GetAllTasks()
        {
            return new ObservableCollection<TaskItem>(_taskList);
        }

        private SolidColorBrush GetRandomColor()
        {
            Random rand = new Random();
            return new SolidColorBrush(Color.FromRgb(
                (byte)rand.Next(256),
                (byte)rand.Next(256),
                (byte)rand.Next(256)));
        }
    }
}