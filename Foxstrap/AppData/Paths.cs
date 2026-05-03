using Foxstrap.Utility;

namespace Foxstrap.AppData
{
    /// <summary>
    /// Централизованные пути — используй это вместо хардкода строк
    /// </summary>
    public static class Paths
    {
        public static string Base => PathUtility.FoxstrapAppData;
        public static string Mods => PathUtility.FoxstrapMods;
        public static string Logs => PathUtility.FoxstrapLogs;
        public static string Settings => PathUtility.FoxstrapSettings;
        public static string Admin => PathUtility.FoxstrapAdmin;
        public static string Roblox => PathUtility.RobloxVersions;
    }
}
