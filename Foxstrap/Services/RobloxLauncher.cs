using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Foxstrap.Services
{
    public static class RobloxLauncher
    {
        // Находим актуальную версию Roblox
        public static string? GetRobloxVersionPath()
        {
            string versionsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "Versions");

            if (!Directory.Exists(versionsPath)) return null;

            foreach (var dir in Directory.GetDirectories(versionsPath))
            {
                if (File.Exists(Path.Combine(dir, "RobloxPlayerBeta.exe")))
                    return dir;
            }
            return null;
        }

        public static string? GetStudioVersionPath()
        {
            string versionsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "Versions");

            if (!Directory.Exists(versionsPath)) return null;

            foreach (var dir in Directory.GetDirectories(versionsPath))
            {
                if (File.Exists(Path.Combine(dir, "RobloxStudioBeta.exe")))
                    return dir;
            }
            return null;
        }

        // Применяем моды — копируем файлы из Mods/ в папку Roblox
        public static void ApplyMods(string robloxVersionPath)
        {
            string modsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foxstrap", "Mods");

            if (!Directory.Exists(modsPath))
            {
                Directory.CreateDirectory(modsPath);
                Logger.Info("Mods folder created (empty)");
                return;
            }

            int modCount = 0;
            foreach (var file in Directory.GetFiles(modsPath, "*", SearchOption.AllDirectories))
            {
                string relative = Path.GetRelativePath(modsPath, file);
                string dest = Path.Combine(robloxVersionPath, relative);
                string? destDir = Path.GetDirectoryName(dest);

                if (destDir != null)
                    Directory.CreateDirectory(destDir);

                try
                {
                    File.Copy(file, dest, overwrite: true);
                    modCount++;
                    Logger.Info($"Mod applied: {relative}");
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Failed to apply mod {relative}: {ex.Message}");
                }
            }

            Logger.Info($"Applied {modCount} mod files");
        }

        // Применяем FastFlags
        public static void ApplyFastFlags(string robloxVersionPath)
        {
            if (!FoxstrapSettings.Get("ManageFastFlags", true))
            {
                Logger.Info("FastFlags management disabled, skipping");
                return;
            }

            string clientSettingsPath = Path.Combine(
                robloxVersionPath, "ClientSettings");
            Directory.CreateDirectory(clientSettingsPath);

            string flagsFile = Path.Combine(clientSettingsPath, "ClientAppSettings.json");

            // Читаем существующие флаги или создаём новые
            var flags = new Dictionary<string, object>();

            if (File.Exists(flagsFile))
            {
                try
                {
                    var existing = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                        File.ReadAllText(flagsFile));
                    if (existing != null)
                        foreach (var kv in existing)
                            flags[kv.Key] = kv.Value.ToString()!;
                }
                catch { }
            }

            // Применяем флаги из настроек
            if (FoxstrapSettings.Get("DisableTelemetry", false))
            {
                flags["FFlagDebugDisableTelemetryEphemeralCounter"] = "True";
                flags["FFlagDebugDisableTelemetryEphemeralStat"] = "True";
                flags["FFlagDebugDisableTelemetryEventIngest"] = "True";
                flags["FFlagDebugDisableTelemetryPoint"] = "True";
                flags["FFlagDebugDisableTelemetryV2Counter"] = "True";
                flags["FFlagDebugDisableTelemetryV2Event"] = "True";
                flags["FFlagDebugDisableTelemetryV2Stat"] = "True";
            }

            if (FoxstrapSettings.Get("Force60FPS", false))
                flags["DFIntTaskSchedulerTargetFps"] = "60";

            if (FoxstrapSettings.Get("IncreaseFarPlane", false))
                flags["FIntRenderFarZ"] = "100000";

            // MSAA
            string msaa = FoxstrapSettings.Get("MSAA", "Автоматически");
            if (msaa != "Автоматически")
            {
                string val = msaa switch
                {
                    "0" => "0", "1" => "1", "2" => "2",
                    "4" => "4", "8" => "8", _ => "4"
                };
                flags["FIntDebugForceMSAASamples"] = val;
            }

            File.WriteAllText(flagsFile, JsonSerializer.Serialize(flags,
                new JsonSerializerOptions { WriteIndented = true }));

            Logger.Info($"FastFlags applied: {flags.Count} flags");
        }

        // Основной метод запуска
        public static async Task<bool> LaunchAsync(
            string? launchUrl = null,
            bool isStudio = false,
            IProgress<string>? progress = null)
        {
            try
            {
                string? versionPath = isStudio
                    ? GetStudioVersionPath()
                    : GetRobloxVersionPath();

                if (versionPath == null)
                {
                    Logger.Error("Roblox version path not found");
                    return false;
                }

                string exeName = isStudio ? "RobloxStudioBeta.exe" : "RobloxPlayerBeta.exe";
                string exePath = Path.Combine(versionPath, exeName);

                progress?.Report("Применение модов...");
                await Task.Run(() => ApplyMods(versionPath));

                progress?.Report("Применение FastFlags...");
                await Task.Run(() => ApplyFastFlags(versionPath));

                progress?.Report($"Запуск {(isStudio ? "Roblox Studio" : "Roblox")}...");
                await Task.Delay(500);

                var psi = new ProcessStartInfo(exePath)
                {
                    UseShellExecute = true
                };

                if (!string.IsNullOrEmpty(launchUrl))
                    psi.Arguments = launchUrl;

                Process.Start(psi);
                Logger.Info($"Launched: {exePath} {launchUrl}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Launch failed", ex);
                return false;
            }
        }
    }
}
