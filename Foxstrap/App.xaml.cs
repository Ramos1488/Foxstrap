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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Logger.Initialize();
            Logger.Info("Foxstrap starting up...");

            ServiceLocator.Initialize();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowCrash(e.Exception);
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                ShowCrash(ex);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Logger.Error("Unobserved task exception", e.Exception);
        }

        private void ShowCrash(Exception ex)
        {
            try
            {
                Dispatcher.Invoke(() => new CrashWindow(ex).ShowDialog());
            }
            catch
            {
                Logger.Fatal("Failed to show crash window", ex);
                Current.Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info($"Foxstrap shutting down (exit code: {e.ApplicationExitCode})");
            base.OnExit(e);
        }
    }
}
