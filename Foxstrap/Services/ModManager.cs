﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace Foxstrap.Services
{
    public static class ModManager
    {
        public static string ModsFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "Mods");

        // Р РµР·РµСЂРІРЅР°СЏ РїР°РїРєР° РѕСЂРёРіРёРЅР°Р»СЊРЅС‹С… С„Р°Р№Р»РѕРІ
        public static string BackupFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "ModBackups");

        public static async Task ApplyAsync(string robloxFolder)
        {
            if (!Directory.Exists(ModsFolder))
            {
                Directory.CreateDirectory(ModsFolder);
                Logger.Info("Mods folder created (empty).");
                return;
            }

            string[] files = Directory.GetFiles(ModsFolder, "*", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Logger.Info("No mods found.");
                return;
            }

            await Task.Run(() =>
            {
                Directory.CreateDirectory(BackupFolder);

                foreach (string modFile in files)
                {
                    string relative = Path.GetRelativePath(ModsFolder, modFile);
                    string dest = Path.Combine(robloxFolder, relative);
                    string backup = Path.Combine(BackupFolder, relative);

                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    Directory.CreateDirectory(Path.GetDirectoryName(backup)!);

                    // Р‘СЌРєР°РїРёРј РѕСЂРёРіРёРЅР°Р» РµСЃР»Рё РµС‰С‘ РЅРµС‚
                    if (File.Exists(dest) && !File.Exists(backup))
                        File.Copy(dest, backup);

                    File.Copy(modFile, dest, overwrite: true);
                    Logger.Info($"Mod applied: {relative}");
                }
            });
        }

        // Р’РѕСЃСЃС‚Р°РЅРѕРІРёС‚СЊ РѕСЂРёРіРёРЅР°Р»С‹ (СѓР±СЂР°С‚СЊ РјРѕРґС‹)
        public static async Task RestoreAsync(string robloxFolder)
        {
            if (!Directory.Exists(BackupFolder)) return;

            await Task.Run(() =>
            {
                foreach (string backup in Directory.GetFiles(BackupFolder, "*", SearchOption.AllDirectories))
                {
                    string relative = Path.GetRelativePath(BackupFolder, backup);
                    string dest = Path.Combine(robloxFolder, relative);
                    File.Copy(backup, dest, overwrite: true);
                    Logger.Info($"Mod restored: {relative}");
                }
            });
        }

        public static void OpenModsFolder()
        {
            Directory.CreateDirectory(ModsFolder);
            System.Diagnostics.Process.Start("explorer.exe", ModsFolder);
        }
    }
}

