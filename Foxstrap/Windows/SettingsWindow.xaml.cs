using Foxstrap.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Foxstrap.Windows
{
    public partial class SettingsWindow : Window
    {
        private Button? _activeNav;

        public SettingsWindow()
        {
            InitializeComponent();
            SelectNav(BtnIntegrations, "Integrations");
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                SelectNav(btn, btn.Tag?.ToString() ?? "");
        }

        private void SelectNav(Button btn, string page)
        {
            if (_activeNav != null)
            {
                _activeNav.Background = Brushes.Transparent;
                _activeNav.Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8));
            }

            _activeNav = btn;
            btn.Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44));
            btn.Foreground = new SolidColorBrush(Color.FromRgb(0xCB, 0xA6, 0xF7));

            ContentPanel.Children.Clear();
            LoadPage(page);
        }

        private void LoadPage(string page)
        {
            switch (page)
            {
                case "Integrations":
                    AddHeader("Интеграции");
                    AddSubHeader("Отслеживание активности в Discord");
                    AddToggle("Включить Discord Rich Presence", "DiscordRPC", false);
                    AddToggle("Показывать название игры", "ShowGameName", true);
                    AddToggle("Показывать время в игре", "ShowTime", true);
                    break;

                case "Launcher":
                    AddHeader("Лаунчер");
                    AddSubHeader("Поведение при запуске");
                    AddToggle("Запускать при старте Windows", "AutoStart", false);
                    AddToggle("Сворачивать в трей после запуска", "MinimizeToTray", true);
                    AddToggle("Показывать окно загрузки", "ShowLoadingWindow", true);
                    break;

                case "Mods":
                    AddHeader("Модификации");
                    AddSubHeader("Управление модами Roblox");
                    AddToggle("Включить загрузку модов", "EnableMods", false);
                    AddInfo("Папка с модами: %LocalAppData%\\Foxstrap\\Mods");
                    break;

                case "FastFlags":
                    AddHeader("Настройки движка");
                    AddSubHeader("FastFlags — изменяют поведение движка Roblox");
                    AddToggle("Отключить телеметрию", "DisableTelemetry", false);
                    AddToggle("Увеличить дальность прорисовки", "IncreaseFarPlane", false);
                    AddToggle("Принудительно 60 FPS", "Force60FPS", false);
                    break;

                case "Appearance":
                    AddHeader("Внешний вид");
                    AddSubHeader("Тема оформления");
                    AddToggle("Использовать акцентный цвет Windows", "UseAccentColor", true);
                    AddToggle("Скруглённые углы", "RoundedCorners", true);
                    break;

                case "About":
                    AddHeader("О Foxstrap");
                    AddInfo("Версия: 1.0.0");
                    AddInfo("Разработчик: Foxi005305");
                    AddInfo("Foxstrap — лаунчер для Roblox на базе .NET 9 и WPF.");
                    break;
            }
        }

        private void AddHeader(string text)
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                Margin = new Thickness(0, 0, 0, 6)
            });
        }

        private void AddSubHeader(string text)
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                Margin = new Thickness(0, 0, 0, 16),
                TextWrapping = TextWrapping.Wrap
            });
        }

        private void AddInfo(string text)
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap
            });
        }

        private void AddToggle(string label, string key, bool defaultValue)
        {
            bool value = FoxstrapSettings.Get(key, defaultValue);

            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var textBlock = new TextBlock
            {
                Text = label,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                VerticalAlignment = VerticalAlignment.Center
            };

            var toggle = new CheckBox
            {
                IsChecked = value,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = key
            };
            toggle.Checked += (s, e) => FoxstrapSettings.Set(key, true);
            toggle.Unchecked += (s, e) => FoxstrapSettings.Set(key, false);

            Grid.SetColumn(toggle, 1);
            grid.Children.Add(textBlock);
            grid.Children.Add(toggle);
            border.Child = grid;
            ContentPanel.Children.Add(border);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FoxstrapSettings.Save();
            Logger.Info("Settings saved");
            MessageBox.Show("Настройки сохранены!", "Foxstrap",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
