﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Foxstrap.Services
{
    public static class IntegrityService
    {
        private static readonly string InstallPath = InstallerService.InstallPath;

        // Список обязательных файлов
        private static readonly List<string> RequiredFiles = new()
        {
            "Foxstrap.exe",
            "Foxstrap.dll",
            "Foxstrap.runtimeconfig.json"
        };

        // Список обязательных папок
        private static readonly List<string> RequiredFolders = new()
        {
            "Assets",
            "Logs",
            "Mods"
        };

        public static bool Check(out List<string> missing)
        {
            missing = new List<string>();

            foreach (var file in RequiredFiles)
            {
                string path = Path.Combine(InstallPath, file);
                if (!File.Exists(path))
                    missing.Add(file);
            }

            foreach (var folder in RequiredFolders)
            {
                string path = Path.Combine(InstallPath, folder);
                if (!Directory.Exists(path))
                    missing.Add(folder + "/");
            }

            return missing.Count == 0;
        }

        public static void Repair()
        {
            Logger.Info("Starting integrity repair...");

            // Восстанавливаем папки
            foreach (var folder in RequiredFolders)
            {
                string path = Path.Combine(InstallPath, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Logger.Info($"Restored folder: {folder}");
                }
            }

            // Копируем недостающие файлы из текущей папки запуска
            string? currentDir = Path.GetDirectoryName(
                System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName);

            if (currentDir == null) return;

            foreach (var file in RequiredFiles)
            {
                string dest = Path.Combine(InstallPath, file);
                string source = Path.Combine(currentDir, file);

                if (!File.Exists(dest) && File.Exists(source))
                {
                    File.Copy(source, dest, true);
                    Logger.Info($"Restored file: {file}");
                }
            }

            // Восстанавливаем Assets
            string assetsSource = Path.Combine(currentDir, "Assets");
            string assetsDest = Path.Combine(InstallPath, "Assets");
            if (Directory.Exists(assetsSource) && !Directory.Exists(assetsDest))
            {
                CopyDirectory(assetsSource, assetsDest);
                Logger.Info("Restored Assets folder");
            }

            Logger.Info("Integrity repair completed");
        }

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
            foreach (var dir in Directory.GetDirectories(source))
                CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
        }
    }
}

