using Foxstrap.Services;
using Foxstrap.Windows;

namespace Foxstrap
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            RobloxInstaller.RegisterUriHandler();
            FastFlagManager.CreateExampleFile();

            if (e.Args.Length > 0 && e.Args[0].StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase))
            {
                var loading = new LoadingWindow(e.Args[0]);
                MainWindow = loading;
                loading.Show();
                return;
            }

            var setup = new SetupWindow();
            MainWindow = setup;
            setup.Show();
        }
    }
}
