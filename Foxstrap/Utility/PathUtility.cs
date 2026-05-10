﻿using System;
using System.IO;

namespace Foxstrap.Utility
{
    public static class PathUtility
    {
        public static string FoxstrapAppData => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap");

        public static string RobloxAppData => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Roblox");

        public static string RobloxVersions => Path.Combine(RobloxAppData, "Versions");
        public static string FoxstrapMods => Path.Combine(FoxstrapAppData, "Mods");
        public static string FoxstrapLogs => Path.Combine(FoxstrapAppData, "Logs");
        public static string FoxstrapSettings => Path.Combine(FoxstrapAppData, "settings.json");
        public static string FoxstrapAdmin => Path.Combine(FoxstrapAppData, "admin.json");
    }
}

