﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Foxstrap.Services
{
    public static class FoxstrapSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "settings.json");

        private static Dictionary<string, JsonElement> _data = new();

        static FoxstrapSettings()
        {
            Load();
        }

        private static void Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    _data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                            ?? new();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to load settings: {ex.Message}");
                _data = new();
            }
        }

        public static T Get<T>(string key, T defaultValue)
        {
            if (_data.TryGetValue(key, out var el))
            {
                try { return el.Deserialize<T>()!; }
                catch { }
            }
            return defaultValue;
        }

        public static void Set<T>(string key, T value)
        {
            _data[key] = JsonSerializer.SerializeToElement(value);
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
                Logger.Info("Settings saved.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save settings: {ex.Message}");
            }
        }
    }
}

