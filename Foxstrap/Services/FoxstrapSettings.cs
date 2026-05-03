using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Foxstrap.Services
{
    public static class FoxstrapSettings
    {
        private static Dictionary<string, JsonElement> _settings = new();
        private static readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "settings.json");

        static FoxstrapSettings() => Load();

        public static void Load()
        {
            try
            {
                if (File.Exists(_path))
                {
                    var json = File.ReadAllText(_path);
                    var doc = JsonDocument.Parse(json);
                    _settings = new();
                    foreach (var prop in doc.RootElement.EnumerateObject())
                        _settings[prop.Name] = prop.Value.Clone();
                    Logger.Info($"Settings loaded: {_settings.Count} keys");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to load settings: {ex.Message}");
                _settings = new();
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                var dict = new Dictionary<string, object?>();
                foreach (var kv in _settings)
                {
                    dict[kv.Key] = kv.Value.ValueKind switch
                    {
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.String => kv.Value.GetString(),
                        JsonValueKind.Number => kv.Value.GetDouble(),
                        _ => kv.Value.ToString()
                    };
                }
                File.WriteAllText(_path, JsonSerializer.Serialize(dict,
                    new JsonSerializerOptions { WriteIndented = true }));
                Logger.Info("Settings saved");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save settings", ex);
            }
        }

        public static T Get<T>(string key, T defaultValue)
        {
            if (!_settings.TryGetValue(key, out var el))
                return defaultValue;
            try
            {
                if (typeof(T) == typeof(bool))
                {
                    bool val = el.ValueKind == JsonValueKind.True;
                    return (T)(object)val;
                }
                if (typeof(T) == typeof(string))
                    return (T)(object)(el.GetString() ?? defaultValue?.ToString() ?? "");
                return el.Deserialize<T>() ?? defaultValue;
            }
            catch { return defaultValue; }
        }

        public static void Set<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            _settings[key] = JsonDocument.Parse(json).RootElement.Clone();
        }
    }
}
