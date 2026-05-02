using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Foxstrap.Services
{
    public static class FoxstrapSettings
    {
        private static Dictionary<string, object> _settings = new();
        private static readonly string _path = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "settings.json");

        static FoxstrapSettings() => Load();

        public static void Load()
        {
            try
            {
                if (File.Exists(_path))
                {
                    var json = File.ReadAllText(_path);
                    _settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
                }
            }
            catch { _settings = new(); }
        }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static T Get<T>(string key, T defaultValue)
        {
            if (_settings.TryGetValue(key, out var val) && val is JsonElement el)
            {
                try { return el.Deserialize<T>() ?? defaultValue; }
                catch { return defaultValue; }
            }
            return defaultValue;
        }

        public static void Set<T>(string key, T value) =>
            _settings[key] = value!;
    }
}
