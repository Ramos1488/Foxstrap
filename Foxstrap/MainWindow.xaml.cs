using Foxstrap.Services;
using Foxstrap.Windows;
using System.Diagnostics;
using System.Windows.Input;

namespace Foxstrap
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
        private void LaunchButton_Click(object sender, RoutedEventArgs e) { new LoadingWindow().Show(); Close(); }
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => new SettingsWindow().Show();
        private void LaunchRoblox_Click(object sender, RoutedEventArgs e) { new LoadingWindow().Show(); Close(); }
        private void LaunchStudio_Click(object sender, RoutedEventArgs e) { new LoadingWindow().Show(); Close(); }
        private void OpenSettings_Click(object sender, RoutedEventArgs e) => new SettingsWindow().Show();
        private void AboutLink_Click(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Foxi005305/Foxstrap") { UseShellExecute = true });
        private void WikiLink_Click(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Foxi005305/Foxstrap/wiki") { UseShellExecute = true });
        private void CommunityLink_Click(object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://discord.gg/foxstrap") { UseShellExecute = true });
    }
}
