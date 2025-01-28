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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Desktop
{
    public partial class Main_empty : Window
    {
        public Main_empty()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow();
            if (addTaskWindow.ShowDialog() == true)
            {
                var mainWindow = new Main();

                var newTask = addTaskWindow.NewTask;
                if (newTask != null)
                {
                    mainWindow.TaskRepository.AddTask(newTask);
                    mainWindow.FilteredTaskList.Add(newTask);
                }
                mainWindow.Show();
                this.Close();
            }
        }
    }
}