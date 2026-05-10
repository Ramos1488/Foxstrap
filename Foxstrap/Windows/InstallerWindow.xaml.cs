using Foxstrap.Services;
using System.Windows.Controls;

namespace Foxstrap.Windows
{
    public partial class InstallerWindow : Window
    {
        public InstallerWindow() => InitializeComponent();

        private void Titlebar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select install folder",
                Filter = "All files|*.*"
            };
            dlg.ShowDialog();
        }

        private void Reset_Click(object sender, RoutedEventArgs e) { }
        private void Back_Click(object sender, RoutedEventArgs e) { }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var progress = new Progress<(int percent, string status)>(p =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (FindName("StatusText") is TextBlock tb) tb.Text = p.status;
                        if (FindName("ProgressBar") is System.Windows.Controls.ProgressBar pb) pb.Value = p.percent;
                    });
                });
                await RobloxInstaller.InstallFromSystemAsync(progress);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Installation failed:\n{ex.Message}", "Foxstrap",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


