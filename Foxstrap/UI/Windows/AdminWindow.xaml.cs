using Foxstrap.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TextBox = System.Windows.Controls.TextBox;

namespace Foxstrap.UI.Windows
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            SetPlaceholder(UsernameInput);
            SetPlaceholder(RobloxIdInput);
            RefreshList();
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void SetPlaceholder(global::System.Windows.Controls.TextBox tb)
        {
            tb.Text = tb.Tag?.ToString() ?? "";
            tb.Foreground = new SolidColorBrush(Color.FromRgb(0x6C, 0x70, 0x86));
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is global::System.Windows.Controls.TextBox tb && tb.Text == tb.Tag?.ToString())
            {
                tb.Text = "";
                tb.Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4));
            }
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is global::System.Windows.Controls.TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
                SetPlaceholder(tb);
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            string robloxId = RobloxIdInput.Text.Trim();

            if (string.IsNullOrEmpty(username) || username == UsernameInput.Tag?.ToString())
            {
                SetStatus("Введите имя пользователя!", "#F38BA8");
                return;
            }

            var role = (RoleCombo.SelectedIndex) switch
            {
                0 => UserRole.Tester,
                1 => UserRole.Moderator,
                2 => UserRole.Admin,
                3 => UserRole.SuperAdmin,
                _ => UserRole.Tester
            };

            if (robloxId == RobloxIdInput.Tag?.ToString()) robloxId = "";

            bool added = AdminService.AddUser(username, robloxId, role, AdminService.MasterUsername);

            if (added)
            {
                SetStatus($"✓ {username} добавлен как {AdminService.RoleToString(role)}", "#A6E3A1");
                SetPlaceholder(UsernameInput);
                SetPlaceholder(RobloxIdInput);
                RoleCombo.SelectedIndex = 0;
                RefreshList();
            }
            else
                SetStatus($"Пользователь {username} уже существует!", "#F38BA8");
        }

        private void RefreshList()
        {
            UserList.Children.Clear();
            var users = AdminService.GetUsers();

            int testers = 0, mods = 0, admins = 0;
            foreach (var user in users)
            {
                switch (user.Role)
                {
                    case UserRole.Tester: testers++; break;
                    case UserRole.Moderator: mods++; break;
                    case UserRole.Admin: case UserRole.SuperAdmin: admins++; break;
                }
                UserList.Children.Add(BuildUserRow(user));
            }

            CountTesters.Text = testers.ToString();
            CountMods.Text = mods.ToString();
            CountAdmins.Text = admins.ToString();
            CountTotal.Text = users.Count.ToString();
        }

        private Border BuildUserRow(FoxstrapUser user)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(16, 10, 16, 10),
                Margin = new Thickness(0, 0, 0, 4)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            grid.Children.Add(new TextBlock
            {
                Text = user.Username, FontSize = 13, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                VerticalAlignment = VerticalAlignment.Center
            });

            var robloxIdText = new TextBlock
            {
                Text = string.IsNullOrEmpty(user.RobloxId) ? "—" : user.RobloxId,
                FontSize = 12, Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(robloxIdText, 1);
            grid.Children.Add(robloxIdText);

            var roleColor = (Color)ColorConverter.ConvertFromString(AdminService.RoleColor(user.Role));
            var roleBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(40, roleColor.R, roleColor.G, roleColor.B)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 3, 8, 3),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = AdminService.RoleToString(user.Role),
                    FontSize = 11, FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(roleColor)
                }
            };
            Grid.SetColumn(roleBadge, 2);
            grid.Children.Add(roleBadge);

            var dateText = new TextBlock
            {
                Text = user.AddedAt.ToString("dd.MM.yyyy"),
                FontSize = 12, Foreground = new SolidColorBrush(Color.FromRgb(0x6C, 0x70, 0x86)),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(dateText, 3);
            grid.Children.Add(dateText);

            var deleteBtn = new Button
            {
                Content = "✕", Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(Color.FromRgb(0xF3, 0x8B, 0xA8)),
                BorderBrush = Brushes.Transparent, Cursor = Cursors.Hand,
                FontSize = 14, VerticalAlignment = VerticalAlignment.Center,
                Tag = user.Username
            };
            deleteBtn.Click += (s, e) =>
            {
                AdminService.RemoveUser(user.Username);
                SetStatus($"✓ {user.Username} удалён", "#F38BA8");
                RefreshList();
            };
            Grid.SetColumn(deleteBtn, 4);
            grid.Children.Add(deleteBtn);

            border.Child = grid;
            return border;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foxstrap", "admin.json");
            if (File.Exists(path))
                Process.Start("explorer.exe", $"/select,\"{path}\"");
            else
                SetStatus("Нет данных для экспорта", "#F38BA8");
        }

        private void SetStatus(string text, string color)
        {
            StatusText.Text = text;
            StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
    }
}

