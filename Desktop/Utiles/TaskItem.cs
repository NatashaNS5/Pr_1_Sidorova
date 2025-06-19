using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Desktop.Utiles
{
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

        public TaskItem(string name, long timestamp, string category, string description = "")
        {
            Name = name;
            Date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime.ToLocalTime(); // Преобразование таймстампа в DateTime
            Category = category;
            Description = description;
        }

        public TaskItem(string name, DateTime date, string category, string description = "") // Для совместимости
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