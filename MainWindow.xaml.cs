using Foxstrap.Services;
using Foxstrap.Windows;
using System.Windows;
using System.Windows.Input;

namespace Foxstrap
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Close();

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            var loading = new LoadingWindow();
            loading.Show();
            Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }
    }
}