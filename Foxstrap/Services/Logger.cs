﻿using System;
using System.IO;

namespace Foxstrap.Services
{
    public static class Logger
    {
        private static readonly string LogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "Logs");

        private static readonly string LogFile =
            Path.Combine(LogDir, $"foxstrap-{DateTime.Now:yyyy-MM-dd}.log");

        static Logger() => Directory.CreateDirectory(LogDir);

        public static string GetLogDirectory() => LogDir;
        public static void Info(string msg)  => Write("INFO ", msg);
        public static void Warn(string msg)  => Write("WARN ", msg);
        public static void Error(string msg) => Write("ERROR", msg);
        public static void Fatal(string msg) => Write("FATAL", msg);

        private static void Write(string level, string msg)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] [{level}] {msg}";
            Console.WriteLine(line);
            try { File.AppendAllText(LogFile, line + Environment.NewLine); }
            catch { }
        }
    }
}
