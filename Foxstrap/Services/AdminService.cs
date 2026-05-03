using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Foxstrap.Services
{
    public enum UserRole
    {
        Tester,
        Moderator,
        Admin,
        SuperAdmin
    }

    public class FoxstrapUser
    {
        public string Username { get; set; } = "";
        public string RobloxId { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.Tester;
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public string AddedBy { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public static class AdminService
    {
        private static List<FoxstrapUser> _users = new();
        private static readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Foxstrap", "admin.json");

        // Захардкоженный супер-админ (ты)
        public static readonly string MasterUsername = "Dima";

        static AdminService() => Load();

        public static List<FoxstrapUser> GetUsers() => new(_users);

        public static void Load()
        {
            try
            {
                if (File.Exists(_path))
                    _users = JsonSerializer.Deserialize<List<FoxstrapUser>>(File.ReadAllText(_path)) ?? new();
            }
            catch { _users = new(); }
        }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static bool AddUser(string username, string robloxId, UserRole role, string addedBy)
        {
            if (_users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;

            _users.Add(new FoxstrapUser
            {
                Username = username,
                RobloxId = robloxId,
                Role = role,
                AddedAt = DateTime.Now,
                AddedBy = addedBy,
                IsActive = true
            });
            Save();
            Logger.Info($"User added: {username} as {role} by {addedBy}");
            return true;
        }

        public static bool RemoveUser(string username)
        {
            int removed = _users.RemoveAll(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (removed > 0) Save();
            return removed > 0;
        }

        public static bool SetRole(string username, UserRole role)
        {
            var user = _users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user is null) return false;
            user.Role = role;
            Save();
            return true;
        }

        public static string RoleToString(UserRole role) => role switch
        {
            UserRole.Tester => "Тестер",
            UserRole.Moderator => "Модератор",
            UserRole.Admin => "Администратор",
            UserRole.SuperAdmin => "Супер-Админ",
            _ => "Неизвестно"
        };

        public static string RoleColor(UserRole role) => role switch
        {
            UserRole.Tester => "#A6E3A1",
            UserRole.Moderator => "#89DCEB",
            UserRole.Admin => "#CBA6F7",
            UserRole.SuperAdmin => "#F38BA8",
            _ => "#CDD6F4"
        };
    }
}
