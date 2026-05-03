using Foxstrap.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Foxstrap.UI.Windows
{
    public partial class InstallerWindow : Window
    {
        private int _currentPage = 0;

        public InstallerWindow()
        {
            InitializeComponent();
            InstallPathText.Text = InstallerService.InstallPath;
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Выберите папку для установки Foxstrap",
                SelectedPath = InstallerService.InstallPath
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                InstallPathText.Text = dialog.SelectedPath;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
            => InstallPathText.Text = InstallerService.InstallPath;

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage == 0)
            {
                GoToPage(1);
            }
            else if (_currentPage == 1)
            {
                GoToPage(2);
                NextButton.IsEnabled = false;
                BackButton.IsEnabled = false;
                await RunInstall();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 0)
                GoToPage(_currentPage - 1);
        }

        private void GoToPage(int page)
        {
            _currentPage = page;

            PageWelcome.Visibility = page == 0 ? Visibility.Visible : Visibility.Collapsed;
            PageInstall.Visibility = page == 1 ? Visibility.Visible : Visibility.Collapsed;
            PageDone.Visibility = page == 2 ? Visibility.Visible : Visibility.Collapsed;

            NavWelcome.Background = new SolidColorBrush(page == 0
                ? Color.FromRgb(0x1E, 0x1E, 0x2E) : Colors.Transparent);
            NavInstall.Background = new SolidColorBrush(page == 1
                ? Color.FromRgb(0x1E, 0x1E, 0x2E) : Colors.Transparent);
            NavDone.Background = new SolidColorBrush(page == 2
                ? Color.FromRgb(0x1E, 0x1E, 0x2E) : Colors.Transparent);

            BackButton.IsEnabled = page > 0;
            NextButton.Content = page == 1 ? "Установить" : "Далее";
        }

        private async Task RunInstall()
        {
            var progress = new Progress<(int percent, string status)>(p =>
            {
                ProgressBar.Value = p.percent;
                StatusLabel.Text = p.status;
                PercentLabel.Text = $"{p.percent}%";
            });

            bool success = await InstallerService.InstallAsync(
                progress,
                DesktopShortcut.IsChecked == true,
                StartMenuShortcut.IsChecked == true);

            if (success)
            {
                StatusLabel.Text = "✓ Foxstrap успешно установлен!";
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xE3, 0xA1));
                NextButton.Content = "Запустить";
                NextButton.IsEnabled = true;
                NextButton.Click -= Next_Click;
                NextButton.Click += (s, e) =>
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                        InstallerService.ExePath) { UseShellExecute = true });
                    Application.Current.Shutdown();
                };
            }
            else
            {
                StatusLabel.Text = "✗ Ошибка установки. Проверьте логи.";
                StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(0xF3, 0x8B, 0xA8));
                BackButton.IsEnabled = true;
            }
        }
    }
}

