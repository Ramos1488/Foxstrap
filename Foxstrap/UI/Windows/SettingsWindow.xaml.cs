using Foxstrap.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Foxstrap.UI.Windows
{
    public partial class SettingsWindow : Window
    {
        private global::System.Windows.Controls.Button? _activeNav;

        public SettingsWindow()
        {
            InitializeComponent();
            SelectNav(BtnIntegrations, "Integrations");
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Close();

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is global::System.Windows.Controls.Button btn)
                SelectNav(btn, btn.Tag?.ToString() ?? "");
        }

        private void SelectNav(global::System.Windows.Controls.Button btn, string page)
        {
            if (_activeNav != null)
            {
                _activeNav.Background = Brushes.Transparent;
                foreach (var child in ((StackPanel)_activeNav.Content).Children)
                    if (child is TextBlock tb && tb.Text != tb.Text.Trim().PadLeft(tb.Text.Length))
                        tb.Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8));
                // reset foreground
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
                    AddDescription("Настройте дополнительную функциональность, чтобы идти параллельно с Roblox.");
                    AddSectionHeader("Отслеживание активности");
                    AddToggle("Включить отслеживание активности",
                        "Разрешить Foxstrap определять, в какую Roblox игру вы играете.", "TrackActivity", false);
                    AddToggle("Запрашивать местоположение сервера",
                        "Во время игры вы сможете увидеть, где находится ваш сервер.", "TrackServer", false);
                    AddToggle("Не выходить в приложение",
                        "Roblox полностью закроется при выходе из игры.", "NoReturnToApp", false);
                    AddSectionHeader("Статус активности Discord (Rich Presence)");
                    AddDescription("Для работы этой функции необходимо включить отслеживание активности и запустить Discord.");
                    AddToggle("Показывать игровую активность",
                        "В вашем профиле Discord будет отображаться игра.", "DiscordRPC", false);
                    AddToggle("Разрешить присоединение через активность",
                        "Разрешает любому человеку присоединиться к игре через Discord.", "DiscordJoin", false);
                    AddToggle("Отображать аккаунт Roblox",
                        "Отображает текущий аккаунт Roblox в вашем профиле Discord.", "DiscordAccount", false);
                    break;

                case "Launcher":
                    AddHeader("Лаунчер");
                    AddDescription("Настройте действия Foxstrap при запуске Roblox.");
                    AddToggle("Запрос на подтверждение при запуске другого окна Roblox",
                        "Предотвратить закрытие запущенной игры из-за случайного запуска.", "MultiInstanceConfirm", false);
                    AddToggle("Фоновые обновления",
                        "Обновлять Roblox в фоновом режиме. Нужно минимум 5 ГБ на диске.", "BackgroundUpdates", false);
                    AddToggle("Принудительная переустановка Roblox",
                        "Roblox будет установлен заново при следующем запуске.", "ForceReinstall", false);
                    break;

                case "Mods":
                    AddHeader("Модификации");
                    AddDescription("Управляйте и применяйте файловые модификации для клиента Roblox.");
                    AddActionButton("Открыть папку с модификациями",
                        "Управляйте пользовательскими модификациями Roblox здесь.", "??", OpenModsFolder);
                    AddSectionHeader("Предустановки");
                    AddDropdown("Курсор мыши", "Выберите между классическими стилями курсора Roblox.",
                        "MouseCursor", new[] { "По умолчанию", "2013 (Угловой)", "2006 (Мультяшный)" }, "По умолчанию");
                    AddToggle("Использовать старый фон редактора аватара",
                        "Возвращает старый фон редактора аватаров до 2020 года.", "OldAvatarBg", false);
                    AddToggle("Эмулировать старые звуки персонажей",
                        "Попытка вернуть звуки персонажей до 2014 года.", "OldSounds", false);
                    AddDropdown("Предпочитаемый тип эмодзи", "Выберите, какой тип эмодзи Roblox будет использовать.",
                        "EmojiType", new[] { "По умолчанию (Twemoji)", "Catmoji", "Windows 11", "Windows 10", "Windows 8" }, "По умолчанию (Twemoji)");
                    break;

                case "FastFlags":
                    AddHeader("Настройки движка");
                    AddDescription("Контролируйте настройку конкретных параметров и функций движка Roblox.");
                    AddToggle("Разрешить Foxstrap управлять Fast Flags",
                        "Отключение предотвратит применение всего настроенного здесь к Roblox.", "ManageFastFlags", true);
                    AddSectionHeader("Предустановки");
                    AddSubSectionHeader("Прорисовка и графика");
                    AddDropdown("Качество сглаживания (MSAA)", "",
                        "MSAA", new[] { "Автоматически", "0", "1", "2", "4", "8" }, "Автоматически");
                    AddToggle("Сохранять качество рендеринга при масштабировании дисплея",
                        "Roblox снижает качество прорисовки в зависимости от настроек масштабирования.", "FixDisplayScale", false);
                    AddDropdown("Качество текстур", "",
                        "TextureQuality", new[] { "Автоматически", "Низкое", "Среднее", "Высокое" }, "Автоматически");
                    AddToggle("Сбросить все по умолчанию", "", "ResetFastFlags", false);
                    break;

                case "Appearance":
                    AddHeader("Внешний вид");
                    AddDescription("Настройте, как должен выглядеть Foxstrap.");
                    AddDropdown("Глобальная тема", "",
                        "Theme", new[] { "По умолчанию системы", "Тёмная", "Светлая" }, "По умолчанию системы");
                    AddSectionHeader("Лаунчер");
                    AddDescription("Вы можете персонализировать всё по своему вкусу.");
                    AddTextInput("Заголовок", "Текст, который отображается в качестве заголовка лаунчера.",
                        "AppTitle", "Foxstrap");
                    AddToggle("Использовать акцентный цвет Windows",
                        "Применять системный акцентный цвет к элементам интерфейса.", "UseAccentColor", true);
                    break;

                case "Shortcuts":
                    AddHeader("Ярлыки");
                    AddDescription("Настройте, как можно легко запустить Foxstrap.");
                    AddToggle("Извлечь значки Roblox в папку",
                        "Позволяет использовать ряд иконок Roblox для ярлыков.", "ExtractIcons", false);
                    AddSectionHeader("Общие");
                    AddDescription("Эти ярлыки вызывают меню запуска с несколькими вариантами.");
                    AddToggleGrid(
                        ("Ярлык на рабочем столе", "DesktopShortcut", true),
                        ("Ярлык в меню «Пуск»", "StartMenuShortcut", true));
                    AddSectionHeader("Функции");
                    AddDescription("Создайте ярлыки для быстрого доступа к определённым функциям.");
                    AddToggleGrid(
                        ("Запустить Roblox", "ShortcutRoblox", false),
                        ("Настройки Foxstrap", "ShortcutSettings", false));
                    break;

                case "FoxstrapApp":
                    AddHeader("Foxstrap");
                    AddDescription("Настройка параметров, связанных с поведением Foxstrap.");
                    AddToggle("Автоматически обновлять Foxstrap",
                        "Foxstrap будет автоматически проверять и обновлять себя при запуске.", "AutoUpdate", true);
                    AddSectionHeader("Диагностика");
                    AddActionButton("Открыть папку с логами",
                        "Просмотреть файлы журнала Foxstrap.", "??", OpenLogsFolder);
                    break;

                case "About":
                    AddHeader("О Foxstrap");
                    AddDescription("Альтернативный лаунчер для Roblox с дополнительными функциями.");
                    var versionBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(16, 14, 16, 14),
                        Margin = new Thickness(0, 8, 0, 16)
                    };
                    var versionPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    var logoBox = new Border
                    {
                        Width = 48, Height = 48, CornerRadius = new CornerRadius(10),
                        Background = new SolidColorBrush(Color.FromRgb(0x27, 0x27, 0x3A)),
                        Margin = new Thickness(0, 0, 14, 0)
                    };
                    logoBox.Child = new TextBlock
                    {
                        Text = "F", FontSize = 24, FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(0xCB, 0xA6, 0xF7)),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    var infoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                    infoPanel.Children.Add(new TextBlock
                    {
                        Text = "Foxstrap  Версия 1.0.0",
                        FontSize = 15, FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
                    });
                    infoPanel.Children.Add(new TextBlock
                    {
                        Text = "Альтернативный лаунчер для Roblox на базе .NET 9",
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8))
                    });
                    versionPanel.Children.Add(logoBox);
                    versionPanel.Children.Add(infoPanel);
                    versionBorder.Child = versionPanel;
                    ContentPanel.Children.Add(versionBorder);

                    var btnGrid = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 16) };
                    btnGrid.Children.Add(MakeLinkButton("⬡", "Репозиторий GitHub", () =>
                        Process.Start(new ProcessStartInfo("https://github.com") { UseShellExecute = true })));
                    btnGrid.Children.Add(MakeLinkButton("?", "Помощь и справка", () =>
                        Process.Start(new ProcessStartInfo("https://github.com") { UseShellExecute = true })));
                    ContentPanel.Children.Add(btnGrid);
                    break;
            }
        }

        private global::System.Windows.Controls.Button MakeLinkButton(string icon, string text, Action onClick)
        {
            var btn = new global::System.Windows.Controls.Button
            {
                Content = icon + "  " + text,
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                BorderBrush = Brushes.Transparent,
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 8, 8),
                Cursor = Cursors.Hand,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                FontSize = 13
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void AddHeader(string text) =>
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text, FontSize = 22, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                Margin = new Thickness(0, 0, 0, 4)
            });

        private void AddDescription(string text) =>
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text, FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                Margin = new Thickness(0, 0, 0, 14), TextWrapping = TextWrapping.Wrap
            });

        private void AddSectionHeader(string text) =>
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text, FontSize = 16, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                Margin = new Thickness(0, 8, 0, 8)
            });

        private void AddSubSectionHeader(string text) =>
            ContentPanel.Children.Add(new TextBlock
            {
                Text = text, FontSize = 13, FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCB, 0xA6, 0xF7)),
                Margin = new Thickness(0, 4, 0, 6)
            });

        private void AddToggle(string label, string description, string key, bool defaultValue)
        {
            bool value = FoxstrapSettings.Get(key, defaultValue);
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 6)
            };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            textPanel.Children.Add(new TextBlock
            {
                Text = label, FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
            });
            if (!string.IsNullOrEmpty(description))
                textPanel.Children.Add(new TextBlock
                {
                    Text = description, FontSize = 11, TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                    Margin = new Thickness(0, 2, 0, 0)
                });

            var toggle = new CheckBox
            {
                Style = (Style)Application.Current.Resources["ToggleSwitch"],
                IsChecked = value, VerticalAlignment = VerticalAlignment.Center, Tag = key
            };
            toggle.Checked += (s, e) => FoxstrapSettings.Set(key, true);
            toggle.Unchecked += (s, e) => FoxstrapSettings.Set(key, false);

            Grid.SetColumn(toggle, 1);
            grid.Children.Add(textPanel);
            grid.Children.Add(toggle);
            border.Child = grid;
            ContentPanel.Children.Add(border);
        }

        private void AddToggleGrid(params (string label, string key, bool defaultValue)[] items)
        {
            var grid = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 6) };
            foreach (var (label, key, def) in items)
            {
                bool value = FoxstrapSettings.Get(key, def);
                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(14, 12, 14, 12),
                    Margin = new Thickness(0, 0, 6, 6)
                };
                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                innerGrid.Children.Add(new TextBlock
                {
                    Text = label, FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                    VerticalAlignment = VerticalAlignment.Center
                });
                var toggle = new CheckBox { IsChecked = value, VerticalAlignment = VerticalAlignment.Center, Style = (Style)Application.Current.Resources["ToggleSwitch"] };
                Grid.SetColumn(toggle, 1);
                toggle.Checked += (s, e) => FoxstrapSettings.Set(key, true);
                toggle.Unchecked += (s, e) => FoxstrapSettings.Set(key, false);
                innerGrid.Children.Add(toggle);
                border.Child = innerGrid;
                grid.Children.Add(border);
            }
            ContentPanel.Children.Add(grid);
        }

        private void AddDropdown(string label, string description, string key, string[] options, string defaultValue)
        {
            string value = FoxstrapSettings.Get(key, defaultValue);
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 6)
            };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });

            var textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            textPanel.Children.Add(new TextBlock
            {
                Text = label, FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
            });
            if (!string.IsNullOrEmpty(description))
                textPanel.Children.Add(new TextBlock
                {
                    Text = description, FontSize = 11, TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                    Margin = new Thickness(0, 2, 0, 0)
                });

            var combo = new ComboBox
            {
                Background = new SolidColorBrush(Color.FromRgb(0x27, 0x27, 0x3A)),
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                BorderBrush = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 0, 0)
            };
            foreach (var opt in options)
                combo.Items.Add(opt);
            combo.SelectedItem = value;
            combo.SelectionChanged += (s, e) =>
            {
                if (combo.SelectedItem is string selected)
                    FoxstrapSettings.Set(key, selected);
            };

            Grid.SetColumn(combo, 1);
            grid.Children.Add(textPanel);
            grid.Children.Add(combo);
            border.Child = grid;
            ContentPanel.Children.Add(border);
        }

        private void AddTextInput(string label, string description, string key, string defaultValue)
        {
            string value = FoxstrapSettings.Get(key, defaultValue);
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 0, 0, 6)
            };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var textPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            textPanel.Children.Add(new TextBlock
            {
                Text = label, FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
            });
            if (!string.IsNullOrEmpty(description))
                textPanel.Children.Add(new TextBlock
                {
                    Text = description, FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                    Margin = new Thickness(0, 2, 0, 0), TextWrapping = TextWrapping.Wrap
                });

            var input = new TextBox
            {
                Text = value,
                Background = new SolidColorBrush(Color.FromRgb(0x27, 0x27, 0x3A)),
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
                BorderBrush = Brushes.Transparent,
                Padding = new Thickness(8, 6, 8, 6),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 0, 0)
            };
            input.TextChanged += (s, e) => FoxstrapSettings.Set(key, input.Text);

            Grid.SetColumn(input, 1);
            grid.Children.Add(textPanel);
            grid.Children.Add(input);
            border.Child = grid;
            ContentPanel.Children.Add(border);
        }

        private void AddActionButton(string label, string description, string icon, Action onClick)
        {
            var btn = new global::System.Windows.Controls.Button
            {
                Background = Brushes.Transparent, BorderBrush = Brushes.Transparent,
                Padding = new Thickness(0), Margin = new Thickness(0, 0, 0, 6),
                Cursor = Cursors.Hand, HorizontalContentAlignment = HorizontalAlignment.Stretch
            };
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x31, 0x32, 0x44)),
                CornerRadius = new CornerRadius(8), Padding = new Thickness(16, 12, 16, 12)
            };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            grid.Children.Add(new TextBlock
            {
                Text = icon + "  ", FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8)),
                VerticalAlignment = VerticalAlignment.Center
            });
            var infoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            infoPanel.Children.Add(new TextBlock
            {
                Text = label, FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4))
            });
            if (!string.IsNullOrEmpty(description))
                infoPanel.Children.Add(new TextBlock
                {
                    Text = description, FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xA6, 0xAD, 0xC8))
                });
            Grid.SetColumn(infoPanel, 1);
            var arrow = new TextBlock
            {
                Text = "›", FontSize = 18,
                Foreground = new SolidColorBrush(Color.FromRgb(0x6C, 0x70, 0x86)),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(arrow, 2);
            grid.Children.Add(infoPanel);
            grid.Children.Add(arrow);
            border.Child = grid;
            btn.Content = border;
            btn.Click += (s, e) => onClick();
            ContentPanel.Children.Add(btn);
        }

        private void OpenModsFolder()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foxstrap", "Mods");
            Directory.CreateDirectory(path);
            Process.Start("explorer.exe", path);
        }

        private void OpenLogsFolder()
        {
            string path = Logger.GetLogDirectory();
            if (Directory.Exists(path))
                Process.Start("explorer.exe", path);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FoxstrapSettings.Save();
            Logger.Info("Settings saved");
            MessageBox.Show("Настройки сохранены!", "Foxstrap",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TestMode_Changed(object sender, RoutedEventArgs e)
        {
            FoxstrapSettings.Set("TestMode", TestModeCheck.IsChecked == true);
        }
    }
}










