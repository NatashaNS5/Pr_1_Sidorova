using Desktop.Repository;
using Desktop.Utiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Desktop.View
{
    public partial class Page2 : Page, INotifyPropertyChanged
    {
        private string _username;
        private ObservableCollection<TaskItem> _filteredTaskList;
        private TaskItem _selectedTask;
        private TaskRepository _taskRepository;
        private bool _isShowingCompletedTasks;
        private bool _isMenuVisible;
        private readonly UserRepository _userRepository;

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
            _taskRepository = new TaskRepository();
            _userRepository = new UserRepository();
            FilteredTaskList = new ObservableCollection<TaskItem>();
            _isMenuVisible = false;

            Loaded += async (s, e) => await InitializePage();

            System.Diagnostics.Debug.WriteLine($"Page2 constructor - UserRepository.CurrentUser: {UserRepository.CurrentUser?.Name ?? "null"}");
            Username = UserRepository.CurrentUser?.Name ?? "Username";
        }

        private async Task InitializePage()
        {
            await _taskRepository.InitializeAsync();
            UpdateMainContentVisibility();
            UpdateCategoryLabels();
            OpenWithFadeIn();

            await LoadUserInfo();
            await LoadProfilePhoto();

            ShowAllTasks_Click(null, null);
        }

        private async Task LoadUserInfo()
        {
            var user = await _userRepository.GetUserInfo();
            Username = user?.Name ?? "Username";
            System.Diagnostics.Debug.WriteLine($"User info loaded: {Username}");
        }

        private async Task LoadProfilePhoto()
        {
            if (UserRepository.CurrentUser?.ImageId != null)
            {
                var photoBytes = await _userRepository.GetUserPhoto(UserRepository.CurrentUser.ImageId);
                if (photoBytes != null)
                {
                    try
                    {
                        using var stream = new MemoryStream(photoBytes);
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        profileImageButtonEmpty.Source = bitmap;
                        profileImageButtonMain.Source = bitmap;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading profile photo: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load profile photo: No bytes received");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No ImageId available");
            }
        }

        private void UpdateMainContentVisibility()
        {
            bool hasTasks = _taskRepository.TaskList.Any() || _taskRepository.CompletedTasks.Any();
            MainGrid.Visibility = hasTasks ? Visibility.Visible : Visibility.Collapsed;
            EmptyGrid.Visibility = hasTasks ? Visibility.Collapsed : Visibility.Visible;

            if (hasTasks)
            {
                ShowMainContent();
            }
            else
            {
                UpdateCategoryLabels();
            }
        }

        private void UpdateCategoryLabels()
        {
            CategoryStackPanel.Children.Clear();

            if (_taskRepository.CategoryColors.Count > 0)
            {
                var allLabel = new Label
                {
                    Content = "Все",
                    Style = (Style)FindResource("LabelStyle"),
                    Foreground = Brushes.Black,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 30
                };
                allLabel.MouseDown += ShowAllTasks_Click;
                CategoryStackPanel.Children.Add(allLabel);
            }

            foreach (var category in _taskRepository.CategoryColors)
            {
                var label = new Label
                {
                    Content = category.Key,
                    Style = (Style)FindResource("LabelStyle"),
                    Foreground = category.Value,
                    Margin = new Thickness(0, 0, 10, 0),
                    Tag = category.Key,
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 30
                };
                label.MouseDown += FilterByCategory_Click;
                CategoryStackPanel.Children.Add(label);
            }

            System.Diagnostics.Debug.WriteLine($"CategoryColors count: {_taskRepository.CategoryColors.Count}");
        }

        private void OpenWithFadeIn()
        {
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private async void CreateFirstTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow();
            AnimateWindow(addTaskWindow);

            if (addTaskWindow.ShowDialog() == true)
            {
                var newTask = addTaskWindow.NewTask;
                if (newTask != null)
                {
                    await _taskRepository.AddTask(newTask);
                    FilteredTaskList.Add(newTask);
                    UpdateCategoryLabels();
                    UpdateMainContentVisibility();
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
                MainGrid.BeginAnimation(OpacityProperty, fadeInAnimation);

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

        private async void CompleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = true;
                await _taskRepository.CompleteTask(SelectedTask);
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" выполнена!");
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
                UpdateMainContentVisibility();
                if (IsShowingCompletedTasks)
                {
                    FilteredTaskList = _taskRepository.CompletedTasks;
                }
                else
                {
                    FilteredTaskList = _taskRepository.GetAllTasks();
                }
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

        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTask != null)
            {
                MessageBox.Show($"Задача \"{SelectedTask.Name}\" удалена!");
                await _taskRepository.RemoveTask(SelectedTask);
                FilteredTaskList.Remove(SelectedTask);
                SelectedTask = null;
                UpdateCategoryLabels();
                UpdateMainContentVisibility();
            }
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow();
            AnimateWindow(addTaskWindow);

            if (addTaskWindow.ShowDialog() == true)
            {
                var newTask = addTaskWindow.NewTask;
                if (newTask != null)
                {
                    await _taskRepository.AddTask(newTask);
                    FilteredTaskList.Add(newTask);
                    UpdateCategoryLabels();
                    UpdateMainContentVisibility();
                }
            }
        }

        private void ProfileImageButtonEmpty_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsMouseOverMenuEmpty())
            {
                menuGridEmpty.Visibility = Visibility.Visible;
                _isMenuVisible = true;
            }
        }

        private void ProfileImageButtonEmpty_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsMouseOverMenuEmpty() && e.OriginalSource is not Button)
            {
                menuGridEmpty.Visibility = Visibility.Hidden;
                _isMenuVisible = false;
            }
        }

        private void MenuGridEmpty_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMenuVisible = true;
            menuGridEmpty.Visibility = Visibility.Visible;
        }

        private void MenuGridEmpty_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsMouseOverImageEmpty())
            {
                menuGridEmpty.Visibility = Visibility.Hidden;
                _isMenuVisible = false;
            }
        }

        private bool IsMouseOverMenuEmpty()
        {
            return menuGridEmpty.IsMouseOver || exitBEmpty.IsMouseOver || profileImageSwitchEmpty.IsMouseOver;
        }

        private bool IsMouseOverImageEmpty()
        {
            return profileImageButtonEmpty.IsMouseOver || ProfileStackPanelEmpty.IsMouseOver;
        }

        private void ExitBEmpty_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    UseShellExecute = true
                }
            };
            process.Start();
            Application.Current.Shutdown();
        }

        private async void ProfileImageSwitchEmpty_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg",
                Title = "Выберите изображение профиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var (success, errorMessage) = await _userRepository.UploadUserPhoto(openFileDialog.FileName);
                if (success)
                {
                    await LoadProfilePhoto();
                    MessageBox.Show("Фото профиля успешно обновлено!");
                }
                else
                {
                    MessageBox.Show($"Ошибка при загрузке фото: {errorMessage}");
                }
            }
        }

        private void ProfileImageButtonMain_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsMouseOverMenuMain())
            {
                menuGridMain.Visibility = Visibility.Visible;
                _isMenuVisible = true;
            }
        }

        private void ProfileImageButtonMain_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsMouseOverMenuMain() && e.OriginalSource is not Button)
            {
                menuGridMain.Visibility = Visibility.Hidden;
                _isMenuVisible = false;
            }
        }

        private void MenuGridMain_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMenuVisible = true;
            menuGridMain.Visibility = Visibility.Visible;
        }

        private void MenuGridMain_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsMouseOverImageMain())
            {
                menuGridMain.Visibility = Visibility.Hidden;
                _isMenuVisible = false;
            }
        }

        private bool IsMouseOverMenuMain()
        {
            return menuGridMain.IsMouseOver || exitBMain.IsMouseOver || profileImageSwitchMain.IsMouseOver;
        }

        private bool IsMouseOverImageMain()
        {
            return profileImageButtonMain.IsMouseOver || ProfileStackPanelMain.IsMouseOver;
        }

        private void ExitBMain_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    UseShellExecute = true
                }
            };
            process.Start();
            Application.Current.Shutdown();
        }

        private async void ProfileImageSwitchMain_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg",
                Title = "Выберите изображение профиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var (success, errorMessage) = await _userRepository.UploadUserPhoto(openFileDialog.FileName);
                if (success)
                {
                    await LoadProfilePhoto();
                    MessageBox.Show("Фото профиля успешно обновлено!");
                }
                else
                {
                    MessageBox.Show($"Ошибка при загрузке фото: {errorMessage}");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}