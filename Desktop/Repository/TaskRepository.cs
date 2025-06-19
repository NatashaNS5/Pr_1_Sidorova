using Desktop.model;
using Desktop.Utiles;
using Desktop.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Desktop.Repository
{
    public class TaskRepository
    {
        private ObservableCollection<TaskItem> _taskList;
        private ObservableCollection<TaskItem> _completedTasks;
        private Dictionary<string, SolidColorBrush> _categoryColors;
        private readonly TodoApiClient _todoApiClient;

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
            _todoApiClient = new TodoApiClient();
        }

        public async Task InitializeAsync()
        {
            try
            {
                var todos = await _todoApiClient.GetTodosAsync();
                _taskList.Clear(); 
                _completedTasks.Clear();
                foreach (var todo in todos)
                {
                    var taskItem = new TaskItem(todo.Title, todo.date, todo.Category, todo.Description)
                    {
                        IsCompleted = todo.IsCompleted
                    };
                    if (todo.IsCompleted)
                    {
                        _completedTasks.Add(taskItem);
                    }
                    else
                    {
                        _taskList.Add(taskItem);
                    }
                    if (!_categoryColors.ContainsKey(taskItem.Category))
                    {
                        _categoryColors[taskItem.Category] = GetRandomColor();
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Initialized with {todos.Length} tasks, CategoryColors count: {_categoryColors.Count}");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки задач", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AddTask(TaskItem task)
        {
            try
            {
                var todoModel = await _todoApiClient.CreateTodoAsync(task.Name, task.Description, task.Category, task.Date, task.IsCompleted);
                var newTask = new TaskItem(todoModel.Title, todoModel.date, todoModel.Category, todoModel.Description)
                {
                    IsCompleted = todoModel.IsCompleted
                };
                if (todoModel.IsCompleted)
                {
                    _completedTasks.Add(newTask);
                }
                else
                {
                    _taskList.Add(newTask);
                }
                if (!_categoryColors.ContainsKey(newTask.Category))
                {
                    _categoryColors[newTask.Category] = GetRandomColor();
                }
                System.Diagnostics.Debug.WriteLine($"Added task, CategoryColors count: {_categoryColors.Count}");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task RemoveTask(TaskItem task)
        {
            try
            {
                var todo = (await _todoApiClient.GetTodosAsync()).FirstOrDefault(t => t.Title == task.Name && t.Category == task.Category && t.date == new DateTimeOffset(task.Date).ToUnixTimeMilliseconds());
                if (todo != null)
                {
                    await _todoApiClient.DeleteTodoAsync(todo.Id);
                    _taskList.Remove(task);
                    if (!_taskList.Any(t => t.Category == task.Category) && !_completedTasks.Any(t => t.Category == task.Category))
                    {
                        _categoryColors.Remove(task.Category);
                    }
                    System.Diagnostics.Debug.WriteLine($"Removed task, CategoryColors count: {_categoryColors.Count}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления задачи", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CompleteTask(TaskItem task)
        {
            if (_taskList.Contains(task))
            {
                _taskList.Remove(task);
                _completedTasks.Add(task);
            }
            System.Diagnostics.Debug.WriteLine($"Completed task, CategoryColors count: {_categoryColors.Count}");
        }

        public ObservableCollection<TaskItem> GetTasksByCategory(string category)
        {
            return new ObservableCollection<TaskItem>(_taskList.Where(task => task.Category == category));
        }

        public ObservableCollection<TaskItem> GetAllTasks()
        {
            return new ObservableCollection<TaskItem>(_taskList);
        }

        public ObservableCollection<TaskItem> CompletedTasksList()
        {
            return new ObservableCollection<TaskItem>(_completedTasks);
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