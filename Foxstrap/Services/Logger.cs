using Serilog;
using System;
using System.IO;

namespace Foxstrap.Services
{
    public static class Logger
    {
        private static ILogger? _logger;

        public static void Initialize()
        {
            string logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foxstrap", "Logs"
            );
            Directory.CreateDirectory(logDirectory);
            string logFile = Path.Combine(logDirectory, "foxstrap-.log");

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    logFile,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Info("Logger initialized");
        }

        public static void Info(string message) => _logger?.Information(message);
        public static void Warn(string message) => _logger?.Warning(message);
        public static void Debug(string message) => _logger?.Debug(message);

        public static void Error(string message, Exception? ex = null)
        {
            if (ex is not null) _logger?.Error(ex, message);
            else _logger?.Error(message);
        }

        public static void Fatal(string message, Exception? ex = null)
        {
            if (ex is not null) _logger?.Fatal(ex, message);
            else _logger?.Fatal(message);
        }

        public static string GetLogDirectory() =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foxstrap", "Logs"
            );
    }
}
