﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Foxstrap.Services
{
    public static class InstallerService
    {
        public static string InstallPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap");

        public static string ExePath => Path.Combine(InstallPath, "Foxstrap.exe");

        public static bool IsInstalled =>
            File.Exists(ExePath) &&
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\Foxstrap") != null;

        public static bool IsRunningFromInstallPath
        {
            get
            {
                string? current = Process.GetCurrentProcess().MainModule?.FileName;
                return current != null &&
                       current.Equals(ExePath, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static async Task<bool> InstallAsync(
            IProgress<(int percent, string status)> progress,
            bool desktopShortcut = true,
            bool startMenuShortcut = true)
        {
            try
            {
                progress.Report((5, "Создание папки установки..."));
                Directory.CreateDirectory(InstallPath);
                Directory.CreateDirectory(Path.Combine(InstallPath, "Logs"));
                Directory.CreateDirectory(Path.Combine(InstallPath, "Mods"));

                await Task.Delay(300);
                progress.Report((20, "Копирование файлов..."));

                string? currentExe = Process.GetCurrentProcess().MainModule?.FileName;
                if (currentExe != null && !currentExe.Equals(ExePath, StringComparison.OrdinalIgnoreCase))
                {
                    string sourceDir = Path.GetDirectoryName(currentExe)!;
                    foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
                    {
                        string destFile = Path.Combine(InstallPath, Path.GetFileName(file));
                        File.Copy(file, destFile, overwrite: true);
                    }

                    string assetsSource = Path.Combine(sourceDir, "Assets");
                    string assetsDest = Path.Combine(InstallPath, "Assets");
                    if (Directory.Exists(assetsSource))
                        CopyDirectory(assetsSource, assetsDest);
                }

                await Task.Delay(400);
                progress.Report((50, "Регистрация протокола roblox://..."));
                RegisterProtocol();

                await Task.Delay(300);
                progress.Report((65, "Регистрация в системе..."));
                RegisterUninstaller();

                await Task.Delay(300);
                progress.Report((80, "Создание ярлыков..."));
                if (desktopShortcut) CreateShortcut(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                if (startMenuShortcut) CreateShortcut(
                    Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.StartMenu), "Programs"));

                await Task.Delay(300);
                progress.Report((100, "Установка завершена!"));
                Logger.Info("Foxstrap installed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Installation failed" + " " + ex);
                return false;
            }
        }

        private static void RegisterProtocol()
        {
            using var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\roblox");
            key.SetValue("", "URL:Roblox Protocol");
            key.SetValue("URL Protocol", "");
            using var iconKey = key.CreateSubKey("DefaultIcon");
            iconKey.SetValue("", $"{ExePath},0");
            using var cmdKey = key.CreateSubKey(@"shell\open\command");
            cmdKey.SetValue("", $"\"{ExePath}\" \"%1\"");

            using var key2 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\roblox-player");
            key2.SetValue("", "URL:Roblox Protocol");
            key2.SetValue("URL Protocol", "");
            using var cmdKey2 = key2.CreateSubKey(@"shell\open\command");
            cmdKey2.SetValue("", $"\"{ExePath}\" \"%1\"");

            Logger.Info("roblox:// protocol registered");
        }

        private static void RegisterUninstaller()
        {
            using var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Uninstall\Foxstrap");
            key.SetValue("DisplayName", "Foxstrap");
            key.SetValue("DisplayVersion", "1.0.0");
            key.SetValue("Publisher", "Foxstrap");
            key.SetValue("InstallLocation", InstallPath);
            key.SetValue("UninstallString", $"\"{ExePath}\" --uninstall");
            key.SetValue("DisplayIcon", ExePath);
            key.SetValue("NoModify", 1);
            key.SetValue("NoRepair", 1);
        }

        private static void CreateShortcut(string directory)
        {
            try
            {
                string shortcutPath = Path.Combine(directory, "Foxstrap.lnk");
                string script = $@"$WS = New-Object -ComObject WScript.Shell; $SC = $WS.CreateShortcut('{shortcutPath}'); $SC.TargetPath = '{ExePath}'; $SC.WorkingDirectory = '{InstallPath}'; $SC.Description = 'Foxstrap - Roblox Launcher'; $SC.Save()";
                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{script}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                })?.WaitForExit();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to create shortcut: {ex.Message}");
            }
        }

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
            foreach (var dir in Directory.GetDirectories(source))
                CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
        }

        public static void Uninstall()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\roblox", false);
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\roblox-player", false);
                Registry.CurrentUser.DeleteSubKeyTree(
                    @"Software\Microsoft\Windows\CurrentVersion\Uninstall\Foxstrap", false);

                string desktop = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Foxstrap.lnk");
                string startMenu = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "Foxstrap.lnk");

                if (File.Exists(desktop)) File.Delete(desktop);
                if (File.Exists(startMenu)) File.Delete(startMenu);

                string batPath = Path.Combine(Path.GetTempPath(), "foxstrap_uninstall.bat");
                File.WriteAllText(batPath, $"@echo off\r\ntimeout /t 2 /nobreak >nul\r\nrmdir /s /q \"{InstallPath}\"\r\ndel \"%~f0\"");
                Process.Start(new ProcessStartInfo
                {
                    FileName = batPath,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                Logger.Info("Foxstrap uninstalled");
            }
            catch (Exception ex)
            {
                Logger.Error("Uninstall failed" + " " + ex);
            }
        }
    }
}


