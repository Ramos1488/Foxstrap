using Foxstrap.Services;
using System.Windows.Controls;

namespace Foxstrap.Windows
{
    public partial class LoadingWindow : Window
    {
        private readonly string? _launchUrl;

        public LoadingWindow(string? launchUrl = null)
        {
            InitializeComponent();
            _launchUrl = launchUrl;
            Closed += (s, e) => Application.Current.Shutdown();
            Loaded += async (s, e) => await RunAsync();
        }

        private async Task RunAsync()
        {
            try
            {
                if (FindName("StatusText") is TextBlock tb)
                    tb.Text = "Checking Roblox...";

                if (await RobloxInstaller.NeedsUpdateAsync())
                {
                    if (FindName("StatusText") is TextBlock tb2)
                        tb2.Text = "Installing Roblox...";
                    await RobloxInstaller.InstallFromSystemAsync();
                }

                if (FindName("StatusText") is TextBlock tb3)
                    tb3.Text = "Launching Roblox...";

                await RobloxLauncher.LaunchAsync(_launchUrl);
                await Task.Delay(1000);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Error("LoadingWindow error: " + ex.Message);
                MessageBox.Show($"Error:\n{ex.Message}", "Foxstrap",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(1);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();
    }
}

