using Foxstrap.Services;
using System.Windows.Controls;

namespace Foxstrap.Windows
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            CheckIfInstalled();
        }

        private void CheckIfInstalled()
        {
            bool installed = System.IO.File.Exists(RobloxInstaller.PlayerExe);
            if (FindName("LaunchButton") is Button lb) lb.IsEnabled = installed;
            if (FindName("StatusText") is TextBlock tb)
                tb.Text = installed
                    ? "Roblox установлен. Можете запустить или настроить лаунчер."
                    : "Roblox не найден. Нажмите кнопку ниже для установки.";
        }

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            if (FindName("InstallButton") is Button ib) ib.IsEnabled = false;
            if (FindName("StatusText") is TextBlock tb) tb.Text = "Установка...";

            var progress = new Progress<(int percent, string status)>(p =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (FindName("StatusText") is TextBlock t) t.Text = p.status;
                    if (FindName("ProgressFill") is Border fill)
                    {
                        double w = ((System.Windows.FrameworkElement)fill.Parent).ActualWidth;
                        fill.Width = w * p.percent / 100.0;
                    }
                });
            });

            try
            {
                await RobloxInstaller.InstallFromSystemAsync(progress);
                if (FindName("StatusText") is TextBlock tb2) tb2.Text = "Установка завершена!";
                if (FindName("LaunchButton") is Button lb) lb.IsEnabled = true;
                if (FindName("InstallButton") is Button ib2) ib2.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.Error("Setup install failed: " + ex.Message);
                MessageBox.Show($"Ошибка установки:\n{ex.Message}", "Foxstrap",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                if (FindName("InstallButton") is Button ib3) ib3.IsEnabled = true;
            }
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            Close();
            new LoadingWindow().Show();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Close();
            new SettingsWindow().Show();
        }
    }
}
