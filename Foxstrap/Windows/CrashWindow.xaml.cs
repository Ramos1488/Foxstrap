using Foxstrap.Services;
using System.Diagnostics;

namespace Foxstrap.Windows
{
    public partial class CrashWindow : Window
    {
        public CrashWindow(Exception ex)
        {
            InitializeComponent();
            Logger.Fatal($"Crash: {ex}");
            if (FindName("ErrorText") is System.Windows.Controls.TextBlock tb)
                tb.Text = ex.ToString();
        }

        private void OpenLogsButton_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(Logger.GetLogDirectory());
            Process.Start("explorer.exe", Logger.GetLogDirectory());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();
    }
}

