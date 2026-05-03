using Foxstrap.Services;
using Foxstrap.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Foxstrap
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Logger.Initialize();
            Logger.Info("Foxstrap starting...");
            Logger.Info($"OS: {Environment.OSVersion} | .NET: {Environment.Version}");

            ServiceLocator.Initialize();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // Обработка удаления
            if (e.Args.Length > 0 && e.Args[0] == "--uninstall")
            {
                new UninstallWindow().Show();
                return;
            }

            // Первый запуск — показываем установщик
            if (!InstallerService.IsInstalled && !InstallerService.IsRunningFromInstallPath)
            {
                new InstallerWindow().Show();
                return;
            }

            // Проверка целостности файлов
            if (InstallerService.IsInstalled)
            {
                if (!IntegrityService.Check(out var missing))
                {
                    Logger.Warn($"Missing files: {string.Join(", ", missing)}");
                    await Task.Run(() => IntegrityService.Repair());
                    Logger.Info("Integrity repaired");
                }
            }

            // Загружаем настройки
            FoxstrapSettings.Load();

            // Показываем главное окно
            new MainWindow().Show();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowCrash(e.Exception);
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex) ShowCrash(ex);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Logger.Error("Unobserved task exception", e.Exception);
        }

        private void ShowCrash(Exception ex)
        {
            try { Dispatcher.Invoke(() => new CrashWindow(ex).ShowDialog()); }
            catch { Logger.Fatal("Failed to show crash window", ex); Current.Shutdown(1); }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info($"Foxstrap shutting down (code: {e.ApplicationExitCode})");
            base.OnExit(e);
        }
    }
}
