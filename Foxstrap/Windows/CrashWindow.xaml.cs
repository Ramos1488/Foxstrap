using Foxstrap.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace Foxstrap.Windows
{
    public partial class CrashWindow : Window
    {
        public CrashWindow(Exception ex)
        {
            InitializeComponent();
            SubtitleText.Text = ex.Message;
            DetailsText.Text = BuildDetails(ex);
            Logger.Fatal("Unhandled exception", ex);
        }

        private static string BuildDetails(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Тип: {ex.GetType().FullName}");
            sb.AppendLine($"Сообщение: {ex.Message}");
            sb.AppendLine();
            sb.AppendLine("Stack trace:");
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException is not null)
            {
                sb.AppendLine();
                sb.AppendLine("--- Inner exception ---");
                sb.AppendLine($"Тип: {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"Сообщение: {ex.InnerException.Message}");
                sb.AppendLine(ex.InnerException.StackTrace);
            }
            return sb.ToString();
        }

        private void OpenLogsButton_Click(object sender, RoutedEventArgs e)
        {
            string logDir = Logger.GetLogDirectory();
            if (Directory.Exists(logDir))
                Process.Start("explorer.exe", logDir);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();
    }
}
