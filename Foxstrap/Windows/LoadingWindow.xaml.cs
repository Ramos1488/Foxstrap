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
            Loaded += async (s, e) => await RunAsync();
        }

        private async Task RunAsync()
        {
            try
            {
                Logger.Info("LoadingWindow: RunAsync started");
                SetStatus("Checking Roblox...");

                if (await RobloxInstaller.NeedsUpdateAsync())
                {
                    SetStatus("Installing Roblox...");
                    await RobloxInstaller.InstallFromSystemAsync();
                }

                SetStatus("Launching Roblox...");
                await RobloxLauncher.LaunchAsync(_launchUrl);
                Logger.Info("LoadingWindow: Roblox launched successfully");

                await Task.Delay(1000);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Error("LoadingWindow error: " + ex);
                MessageBox.Show($"Error:\n{ex.Message}", "Foxstrap",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(1);
            }
        }

        private void SetStatus(string text)
        {
            if (FindName("StatusText") is TextBlock tb) tb.Text = text;
            Logger.Info("Status: " + text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();
    }
}
