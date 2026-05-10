using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Foxstrap.Services
{
    public static class RobloxLauncher
    {
        public static async Task LaunchAsync(string? launchUrl = null)
        {
            Logger.Info($"LaunchAsync called. URL={launchUrl ?? "none"}");

            await FastFlagManager.ApplyAsync(RobloxInstaller.RobloxFolder);

            if (FoxstrapSettings.Get("EnableMods", false))
                await ModManager.ApplyAsync(RobloxInstaller.RobloxFolder);

            string args = string.IsNullOrEmpty(launchUrl) ? "--app" : launchUrl;
            Logger.Info($"Starting: {RobloxInstaller.PlayerExe} {args}");

            var psi = new ProcessStartInfo
            {
                FileName         = RobloxInstaller.PlayerExe,
                Arguments        = args,
                UseShellExecute  = false,
                WorkingDirectory = RobloxInstaller.RobloxFolder
            };

            var proc = Process.Start(psi)
                ?? throw new Exception("Failed to start RobloxPlayerBeta.exe");

            if (FoxstrapSettings.Get("DiscordRPC", false))
                DiscordRpcManager.Start(proc);

            Logger.Info($"Roblox started. PID={proc.Id}");
        }
    }
}
