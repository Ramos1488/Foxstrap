﻿using DiscordRPC;
using System;
using System.Diagnostics;

namespace Foxstrap.Services
{
    public static class DiscordRpcManager
    {
        private static DiscordRpcClient? _client;

        public static void Start(Process robloxProcess)
        {
            try
            {
                _client = new DiscordRpcClient("1012053089095282699"); // Roblox App ID
                _client.Initialize();

                _client.SetPresence(new RichPresence
                {
                    Details = "РРіСЂР°РµС‚ РІ Roblox",
                    State = "Р§РµСЂРµР· Foxstrap",
                    Assets = new Assets
                    {
                        LargeImageKey = "roblox",
                        LargeImageText = "Roblox",
                        SmallImageKey = "foxstrap",
                        SmallImageText = "Foxstrap Launcher"
                    },
                    Timestamps = new Timestamps(DateTime.UtcNow)
                });

                Logger.Info("Discord RPC started.");

                // РћСЃС‚Р°РЅР°РІР»РёРІР°РµРј РєРѕРіРґР° Roblox Р·Р°РєСЂС‹РІР°РµС‚СЃСЏ
                robloxProcess.EnableRaisingEvents = true;
                robloxProcess.Exited += (_, _) => Stop();
            }
            catch (Exception ex)
            {
                Logger.Warn($"Discord RPC failed: {ex.Message}");
            }
        }

        public static void UpdateGame(string gameName)
        {
            _client?.UpdateDetails($"РРіСЂР°РµС‚ РІ {gameName}");
        }

        public static void Stop()
        {
            try
            {
                _client?.ClearPresence();
                _client?.Dispose();
                _client = null;
                Logger.Info("Discord RPC stopped.");
            }
            catch { }
        }
    }
}

