using Foxstrap.Services;

namespace Foxstrap.Windows
{
    public partial class UninstallWindow : Window
    {
        public UninstallWindow() => InitializeComponent();

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Remove Roblox installed by Foxstrap?", "Foxstrap",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                RobloxInstaller.Uninstall();
                MessageBox.Show("Done.", "Foxstrap", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}

