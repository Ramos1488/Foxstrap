using Foxstrap.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Foxstrap.Windows
{
    public partial class UninstallWindow : Window
    {
        public UninstallWindow()
        {
            InitializeComponent();
            InstallPathText.Text = $"Установлен в: {InstallerService.InstallPath}";
        }

        private async void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            bool keepData = KeepData.IsChecked == true;

            UninstallBtn.IsEnabled = false;
            CancelBtn.IsEnabled = false;
            UninstallProgress.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                Dispatcher.Invoke(() => UninstallProgress.Value = 15);

                // Удаляем протоколы
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\roblox", false);
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\roblox-player", false);

                Dispatcher.Invoke(() => UninstallProgress.Value = 40);

                // Удаляем запись установки
                Registry.CurrentUser.DeleteSubKeyTree(
                    @"Software\Microsoft\Windows\CurrentVersion\Uninstall\Foxstrap", false);

                Dispatcher.Invoke(() => UninstallProgress.Value = 60);

                // Удаляем ярлыки
                string desktop = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Foxstrap.lnk");
                string startMenu = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "Foxstrap.lnk");
                if (File.Exists(desktop)) File.Delete(desktop);
                if (File.Exists(startMenu)) File.Delete(startMenu);

                Dispatcher.Invoke(() => UninstallProgress.Value = 80);

                if (!keepData)
                {
                    // Удаляем всю папку
                    string bat = System.IO.Path.Combine(
                        System.IO.Path.GetTempPath(), "foxstrap_uninstall.bat");
                    File.WriteAllText(bat,
                        $"@echo off\r\ntimeout /t 2 /nobreak >nul\r\nrmdir /s /q \"{InstallerService.InstallPath}\"\r\ndel \"%~f0\"");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = bat,
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    });
                }
                else
                {
                    // Удаляем только exe и dll, оставляем Mods, Logs, settings
                    string installPath = InstallerService.InstallPath;
                    string bat = System.IO.Path.Combine(
                        System.IO.Path.GetTempPath(), "foxstrap_uninstall.bat");

                    string batContent = "@echo off\r\ntimeout /t 2 /nobreak >nul\r\n";
                    foreach (var file in Directory.GetFiles(installPath, "*.exe"))
                        batContent += $"del /f /q \"{file}\"\r\n";
                    foreach (var file in Directory.GetFiles(installPath, "*.dll"))
                        batContent += $"del /f /q \"{file}\"\r\n";
                    foreach (var file in Directory.GetFiles(installPath, "*.json"))
                    {
                        if (!file.Contains("settings") && !file.Contains("admin"))
                            batContent += $"del /f /q \"{file}\"\r\n";
                    }
                    batContent += $"del \"%~f0\"";
                    File.WriteAllText(bat, batContent);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = bat,
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    });
                }

                Dispatcher.Invoke(() => UninstallProgress.Value = 100);
            });

            // Показываем успех
            TitleText.Text = "✓ Foxstrap удалён";
            TitleText.Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xE3, 0xA1));
            DescText.Text = keepData
                ? "Foxstrap удалён. Ваши настройки и моды сохранены и будут восстановлены при переустановке."
                : "Foxstrap полностью удалён с вашей системы.";
            KeepData.Visibility = Visibility.Collapsed;
            InstallPathText.Visibility = Visibility.Collapsed;

            UninstallBtn.Content = "Закрыть";
            UninstallBtn.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E));
            UninstallBtn.Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4));
            UninstallBtn.IsEnabled = true;
            CancelBtn.Visibility = Visibility.Collapsed;
            UninstallBtn.Click -= Uninstall_Click;
            UninstallBtn.Click += (s, ev) => Application.Current.Shutdown();

            Logger.Info($"Foxstrap uninstalled (keepData={keepData})");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
