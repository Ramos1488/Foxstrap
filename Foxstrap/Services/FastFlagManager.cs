using System.Collections.Generic;
using System.Text.Json;

namespace Foxstrap.Services
{
    public static class FastFlagManager
    {
        private static readonly string CustomFile =
            Path.Combine(RobloxInstaller.BaseFolder, "CustomFastFlags.json");

        public static async Task ApplyAsync(string robloxFolder)
        {
            var flags = new Dictionary<string, object>();

            if (FoxstrapSettings.Get("DisableTelemetry", false))
            {
                flags["FFlagDebugDisableTelemetryV2"] = true;
                flags["DFIntTelemetryRolloutPercent"] = 0;
            }
            if (FoxstrapSettings.Get("Force60FPS", false))
                flags["DFIntTaskSchedulerTargetFps"] = 60;
            if (FoxstrapSettings.Get("IncreaseFarPlane", false))
                flags["FFlagRenderFarPlaneMultiplier"] = 8;
            if (FoxstrapSettings.Get("DisableShadows", false))
                flags["FIntRenderShadowIntensity"] = 0;
            if (FoxstrapSettings.Get("DisablePostFX", false))
                flags["FFlagDisablePostFx"] = true;

            if (File.Exists(CustomFile))
            {
                try
                {
                    var custom = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                        await File.ReadAllTextAsync(CustomFile));
                    if (custom != null)
                        foreach (var (k, v) in custom)
                            flags[k] = v;
                }
                catch (Exception ex) { Logger.Warn("CustomFastFlags error: " + ex.Message); }
            }

            if (flags.Count == 0) return;

            string dir = Path.Combine(robloxFolder, "ClientSettings");
            Directory.CreateDirectory(dir);
            await File.WriteAllTextAsync(
                Path.Combine(dir, "ClientAppSettings.json"),
                JsonSerializer.Serialize(flags, new JsonSerializerOptions { WriteIndented = true }));

            Logger.Info($"Applied {flags.Count} FastFlag(s).");
        }

        public static void CreateExampleFile()
        {
            if (File.Exists(CustomFile)) return;
            Directory.CreateDirectory(RobloxInstaller.BaseFolder);
            File.WriteAllText(CustomFile, "{\n\n}\n");
        }
    }
}

