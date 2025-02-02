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
    public partial class Main_empty : Window
    {
        public Main_empty()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow();
            AnimateWindow(addTaskWindow);

            if (addTaskWindow.ShowDialog() == true)
            {
                var mainWindow = new Main();

                var newTask = addTaskWindow.NewTask;
                if (newTask != null)
                {
                    mainWindow.TaskRepository.AddTask(newTask);
                    mainWindow.FilteredTaskList.Add(newTask);
                }

                var storyboard = new Storyboard();
                var animation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };
                Storyboard.SetTarget(animation, this);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Window.OpacityProperty));
                storyboard.Children.Add(animation);

                storyboard.Completed += (s, args) =>
                {
                    mainWindow.Show();
                    this.Close();
                };
                storyboard.Begin();
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
}