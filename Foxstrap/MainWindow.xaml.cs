using Foxstrap.Services;
using Foxstrap.ViewModels;
using Foxstrap.Windows;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Foxstrap
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Get<MainViewModel>();
            ApplyAccentColor();
        }

        private void ApplyAccentColor()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");
                if (key?.GetValue("AccentColor") is int colorInt)
                {
                    byte r = (byte)(colorInt & 0xFF);
                    byte g = (byte)((colorInt >> 8) & 0xFF);
                    byte b = (byte)((colorInt >> 16) & 0xFF);
                    var accent = new SolidColorBrush(Color.FromArgb(255, r, g, b));
                    var darkAccent = new SolidColorBrush(Color.FromArgb(40, r, g, b));
                    LogoBorder.Background = darkAccent;
                    LogoText.Foreground = accent;
                }
            }
            catch (Exception ex) { Logger.Warn($"Accent color error: {ex.Message}"); }
        }

        private string? FindRobloxPath()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "Versions");
            if (!Directory.Exists(folder)) return null;
            foreach (var dir in Directory.GetDirectories(folder))
            {
                string exe = Path.Combine(dir, "RobloxPlayerBeta.exe");
                if (File.Exists(exe)) return exe;
            }
            return null;
        }

        private string? FindRobloxStudioPath()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "Versions");
            if (!Directory.Exists(folder)) return null;
            foreach (var dir in Directory.GetDirectories(folder))
            {
                string exe = Path.Combine(dir, "RobloxStudioBeta.exe");
                if (File.Exists(exe)) return exe;
            }
            return null;
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private async void LaunchRoblox_Click(object sender, RoutedEventArgs e)
        {
            string? path = FindRobloxPath();
            if (path is null)
            {
                MessageBox.Show("Не удалось найти Roblox. Убедитесь что он установлен.",
                    "Foxstrap", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var loading = new LoadingWindow("Запускаем Roblox...");
            loading.Show();
            Hide();

            await Task.Delay(1500);

            if (!loading.Cancelled)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    Logger.Info($"Roblox launched: {path}");
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to launch Roblox", ex);
                    MessageBox.Show("Не удалось запустить Roblox.", "Foxstrap",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            loading.Close();
            Show();
        }

        private async void LaunchStudio_Click(object sender, RoutedEventArgs e)
        {
            string? path = FindRobloxStudioPath();
            if (path is null)
            {
                MessageBox.Show("Не удалось найти Roblox Studio.",
                    "Foxstrap", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var loading = new LoadingWindow("Запускаем Roblox Studio...");
            loading.Show();
            Hide();

            await Task.Delay(1500);

            if (!loading.Cancelled)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    Logger.Info($"Roblox Studio launched: {path}");
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to launch Roblox Studio", ex);
                }
            }

            loading.Close();
            Show();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            settings.ShowDialog();
        }

        private void AboutLink_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Foxstrap v1.0.0\nЛаунчер для Roblox на базе .NET 9",
                "О Foxstrap", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void WikiLink_Click(object sender, MouseButtonEventArgs e)
            => Process.Start(new ProcessStartInfo("https://github.com") { UseShellExecute = true });

        private void CommunityLink_Click(object sender, MouseButtonEventArgs e)
            => Process.Start(new ProcessStartInfo("https://www.roblox.com") { UseShellExecute = true });
    }
}
