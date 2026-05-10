using Foxstrap.Services;
using Foxstrap.Windows;
using System;
using System.Windows;

namespace Foxstrap
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RobloxInstaller.RegisterUriHandler();
            FastFlagManager.CreateExampleFile();

            string? launchUrl = null;

            if (e.Args.Length > 0 && e.Args[0].StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase))
            {
                launchUrl = e.Args[0];
                Logger.Info($"Launched via URI: {launchUrl}");
                var loading = new LoadingWindow(launchUrl);
                loading.Show();
                return;
            }

            Logger.Info("Launched normally.");
            var settings = new SettingsWindow();
            MainWindow = settings;
            settings.Show();
        }
    }
}