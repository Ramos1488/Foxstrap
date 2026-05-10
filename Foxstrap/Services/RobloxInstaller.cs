﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Foxstrap.Services
{
    public static class RobloxInstaller
    {
        private static readonly HttpClient _http = new();

        public static string BaseFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Foxstrap");

        public static string RobloxFolder => Path.Combine(BaseFolder, "Roblox");
        public static string VersionFile  => Path.Combine(RobloxFolder, "version.txt");
        public static string PlayerExe    => Path.Combine(RobloxFolder, "RobloxPlayerBeta.exe");

        public static async Task<string> GetLatestVersionAsync()
        {
            string json = await _http.GetStringAsync(
                "https://clientsettingscdn.roblox.com/v1/client-version/WindowsPlayer");
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("clientVersionUpload").GetString()!;
        }

        public static string? GetInstalledVersion()
            => File.Exists(VersionFile) ? File.ReadAllText(VersionFile).Trim() : null;

        public static async Task<bool> NeedsUpdateAsync()
        {
            if (!File.Exists(PlayerExe)) return true;
            try
            {
                string latest = await GetLatestVersionAsync();
                return GetInstalledVersion() != latest;
            }
            catch { return false; }
        }

        // Копирует из официальной папки Roblox в Foxstrap\Roblox
        public static async Task InstallFromSystemAsync(IProgress<(int, string)>? progress = null)
        {
            progress?.Report((0, "Looking for Roblox installation..."));

            string versionsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "Versions");

            if (!Directory.Exists(versionsPath))
                throw new Exception("Roblox is not installed. Please install Roblox first from roblox.com");

            var dirs = new DirectoryInfo(versionsPath).GetDirectories();
            if (dirs.Length == 0)
                throw new Exception("No Roblox version found in " + versionsPath);

            // Берём самую новую папку
            var latest = dirs[0];
            foreach (var d in dirs)
                if (d.LastWriteTime > latest.LastWriteTime)
                    latest = d;

            progress?.Report((5, $"Found Roblox: {latest.Name}"));

            // Проверяем что это рабочая версия
            string playerExe = Path.Combine(latest.FullName, "RobloxPlayerBeta.exe");
            if (!File.Exists(playerExe))
                throw new Exception($"RobloxPlayerBeta.exe not found in {latest.Name}");

            // Копируем
            Directory.CreateDirectory(RobloxFolder);
            string[] files = Directory.GetFiles(latest.FullName, "*", SearchOption.AllDirectories);
            int total = files.Length;
            int done = 0;

            await Task.Run(() =>
            {
                foreach (string file in files)
                {
                    string rel  = Path.GetRelativePath(latest.FullName, file);
                    string dest = Path.Combine(RobloxFolder, rel);
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                        File.Copy(file, dest, overwrite: true);
                    }
                    catch { }
                    done++;
                    if (done % 20 == 0)
                        progress?.Report((5 + done * 90 / total, $"Copying: {rel}"));
                }
            });

            await File.WriteAllTextAsync(VersionFile, latest.Name);
            progress?.Report((100, "Done!"));
            Logger.Info($"Roblox {latest.Name} installed from system.");
        }

        public static void RegisterUriHandler()
        {
            try
            {
                string exe = Process.GetCurrentProcess().MainModule!.FileName;
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\roblox-player");
                key.SetValue("", "URL:Roblox Player Protocol");
                key.SetValue("URL Protocol", "");
                using var icon = key.CreateSubKey("DefaultIcon");
                icon.SetValue("", $"\"{exe}\",0");
                using var cmd = key.CreateSubKey(@"shell\open\command");
                cmd.SetValue("", $"\"{exe}\" \"%1\"");
                Logger.Info("Registered roblox-player:// URI handler.");
            }
            catch (Exception ex) { Logger.Warn("URI handler failed: " + ex.Message); }
        }

        public static void Uninstall()
        {
            if (Directory.Exists(RobloxFolder))
                Directory.Delete(RobloxFolder, recursive: true);
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\roblox-player", false);
            Logger.Info("Roblox uninstalled.");
        }
    }
}
