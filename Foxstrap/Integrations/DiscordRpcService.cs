using Foxstrap.Services;
using System;

namespace Foxstrap.Integrations
{
    /// <summary>
    /// Discord Rich Presence интеграция.
    /// Для полной реализации добавьте пакет: dotnet add package DiscordRichPresence
    /// </summary>
    public static class DiscordRpcService
    {
        private static bool _isRunning = false;
        private const string AppId = "1234567890"; // Замени на свой Discord App ID

        public static void Start()
        {
            if (!FoxstrapSettings.Get("DiscordRPC", false))
            {
                Logger.Info("Discord RPC disabled in settings");
                return;
            }

            try
            {
                _isRunning = true;
                Logger.Info("Discord RPC started");
                // TODO: инициализация Discord RPC клиента
                // var client = new DiscordRpcClient(AppId);
                // client.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to start Discord RPC", ex);
            }
        }

        public static void UpdatePresence(string details, string state = "")
        {
            if (!_isRunning) return;
            Logger.Info($"Discord RPC: {details} | {state}");
            // TODO: client.SetPresence(...)
        }

        public static void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            Logger.Info("Discord RPC stopped");
            // TODO: client.Dispose();
        }
    }
}
