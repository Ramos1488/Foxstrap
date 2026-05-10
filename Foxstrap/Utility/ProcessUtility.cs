﻿using System.Diagnostics;
using System.Linq;

namespace Foxstrap.Utility
{
    public static class ProcessUtility
    {
        public static bool IsRobloxRunning() =>
            Process.GetProcessesByName("RobloxPlayerBeta").Any();

        public static bool IsStudioRunning() =>
            Process.GetProcessesByName("RobloxStudioBeta").Any();

        public static void KillRoblox()
        {
            foreach (var p in Process.GetProcessesByName("RobloxPlayerBeta"))
                p.Kill();
        }

        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}

