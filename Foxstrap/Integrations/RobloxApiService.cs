﻿using Foxstrap.Services;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Foxstrap.Integrations
{
    public static class RobloxApiService
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("https://api.roblox.com/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        /// <summary>Получить текущую версию Roblox клиента</summary>
        public static async Task<string?> GetLatestVersionAsync()
        {
            try
            {
                string url = "https://clientsettingscdn.roblox.com/v2/client-version/WindowsPlayer";
                var response = await _http.GetStringAsync(url);
                var doc = JsonDocument.Parse(response);
                return doc.RootElement.GetProperty("clientVersionUpload").GetString();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to get Roblox version" + " " + ex);
                return null;
            }
        }

        /// <summary>Получить информацию о пользователе по Roblox ID</summary>
        public static async Task<RobloxUser?> GetUserAsync(long userId)
        {
            try
            {
                string url = $"https://users.roblox.com/v1/users/{userId}";
                var response = await _http.GetStringAsync(url);
                return JsonSerializer.Deserialize<RobloxUser>(response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get Roblox user {userId}" + " " + ex);
                return null;
            }
        }
    }

    public class RobloxUser
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
    }
}


