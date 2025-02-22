using Desktop.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Repository
{
    public class TaskRepository
    {
        private ObservableCollection<TaskItem> _taskList;
        private ObservableCollection<TaskItem> _completedTasks;

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

        public TaskRepository()
        {
            _taskList = new ObservableCollection<TaskItem>();
            _completedTasks = new ObservableCollection<TaskItem>();
        }

        public void AddTask(TaskItem task)
        {
            _taskList.Add(task);
        }

        public void RemoveTask(TaskItem task)
        {
            _taskList.Remove(task);
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
    }
}