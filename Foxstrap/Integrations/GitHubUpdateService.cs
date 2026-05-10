﻿using Foxstrap.Services;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Foxstrap.Integrations
{
    public class GitHubRelease
    {
        public string tag_name { get; set; } = "";
        public string name { get; set; } = "";
        public string body { get; set; } = "";
        public bool prerelease { get; set; }
        public string html_url { get; set; } = "";
    }

    public static class GitHubUpdateService
    {
        private const string CurrentVersion = "1.0.0";
        private const string RepoOwner = "Ramos1488";
        private const string RepoName = "Foxstrap";

        private static readonly HttpClient _http = new();

        static GitHubUpdateService()
        {
            _http.DefaultRequestHeaders.Add("User-Agent", "Foxstrap");
        }

        public static async Task<GitHubRelease?> CheckForUpdateAsync()
        {
            try
            {
                string url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
                string json = await _http.GetStringAsync(url);
                var release = JsonSerializer.Deserialize<GitHubRelease>(json);

                if (release == null) return null;

                string latest = release.tag_name.TrimStart('v');
                if (Version.TryParse(latest, out var latestVer) &&
                    Version.TryParse(CurrentVersion, out var currentVer) &&
                    latestVer > currentVer)
                {
                    Logger.Info($"Update available: {latest}");
                    return release;
                }

                Logger.Info("Foxstrap is up to date");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Warn($"Update check failed: {ex.Message}");
                return null;
            }
        }

        public static async Task DownloadAndInstallAsync(string downloadUrl, IProgress<int>? progress = null)
        {
            try
            {
                Logger.Info($"Downloading update from: {downloadUrl}");
                // TODO: скачать новый exe и запустить установщик
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Logger.Error("Update download failed" + " " + ex);
            }
        }
    }
}


