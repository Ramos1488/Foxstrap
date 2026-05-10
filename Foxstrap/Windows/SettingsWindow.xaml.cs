using Foxstrap.Services;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Foxstrap.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            if (FindName("NavIntegrations") is RadioButton rb) rb.IsChecked = true;
            if (FindName("TestModeCheck") is CheckBox cb)
                cb.IsChecked = FoxstrapSettings.Get("TestMode", false);
        }

        private void Nav_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb) LoadPage(rb.Tag?.ToString() ?? "");
        }

        private void LoadPage(string page)
        {
            if (FindName("ContentPanel") is not StackPanel panel) return;
            panel.Children.Clear();

            switch (page)
            {
                case "Integrations":
                    AddHeader(panel, "Интеграции", "Настройте дополнительную функциональность для Roblox.");
                    AddSection(panel, "Статус активности Discord (Rich Presence)");
                    AddToggle(panel, "Включить Discord Rich Presence", "Отображает вашу игровую активность в Discord.", "DiscordRPC", false);
                    AddToggle(panel, "Показывать название игры", "Отображает текущую игру в статусе.", "ShowGameName", true);
                    AddToggle(panel, "Показывать время в игре", "Отображает время, проведённое в игре.", "ShowTime", true);
                    AddToggle(panel, "Разрешить присоединение через активность", "Друзья в Discord смогут присоединиться к вашей игре.", "AllowJoining", false);
                    break;

                case "Launcher":
                    AddHeader(panel, "Лаунчер", "Настройте поведение Foxstrap при запуске.");
                    AddToggle(panel, "Показывать окно загрузки", "Показывает прогресс при запуске Roblox.", "ShowLoadingWindow", true);
                    AddToggle(panel, "Запускать при старте Windows", "Запускать Foxstrap вместе с Windows.", "AutoStart", false);
                    AddToggle(panel, "Фоновые обновления", "Обновлять Roblox в фоновом режиме.", "BackgroundUpdates", true);
                    AddToggle(panel, "Запрос при нескольких копиях", "Спрашивать перед открытием второго Roblox.", "MultiInstancePrompt", true);
                    break;

                case "Mods":
                    AddHeader(panel, "Модификации", "Управляйте модификациями клиента Roblox.");
                    AddToggle(panel, "Включить моды", "Разрешить Foxstrap применять файловые моды.", "EnableMods", false);
                    AddAction(panel, "Открыть папку модов", "Поместите файлы модов сюда.", () => ModManager.OpenModsFolder());
                    AddSection(panel, "Предустановки");
                    AddToggle(panel, "Старый курсор (2013)", "Возвращает классический угловой курсор.", "OldCursor", false);
                    AddToggle(panel, "Старые звуки персонажа", "Возвращает звуки персонажа до 2014 года.", "OldSounds", false);
                    AddToggle(panel, "Старый фон редактора аватара", "Возвращает фон редактора аватара до 2020 года.", "OldAvatarBg", false);
                    AddSection(panel, "Кастомный курсор");
                    AddInfo(panel, "Поместите .cur или .ani файлы в: %LocalAppData%\\Foxstrap\\Cursors\\");
                    AddAction(panel, "Открыть папку курсоров", "", () =>
                    {
                        string dir = Path.Combine(RobloxInstaller.BaseFolder, "Cursors");
                        Directory.CreateDirectory(dir);
                        Process.Start("explorer.exe", dir);
                    });
                    AddSection(panel, "Кастомный шрифт");
                    AddInfo(panel, "Выберите .ttf или .otf файл шрифта с вашего устройства.");
                    AddFontPicker(panel);
                    break;

                case "FastFlags":
                    AddHeader(panel, "Настройки движка", "Управляйте параметрами движка Roblox.");
                    AddSection(panel, "Производительность");
                    AddToggle(panel, "Отключить телеметрию", "Предотвращает сбор данных Roblox.", "DisableTelemetry", false);
                    AddToggle(panel, "Принудительно 60 FPS", "Ограничивает частоту кадров до 60 FPS.", "Force60FPS", false);
                    AddToggle(panel, "Увеличить дальность прорисовки", "Увеличивает максимальную дальность отрисовки.", "IncreaseFarPlane", false);
                    AddSection(panel, "Графика");
                    AddToggle(panel, "Отключить тени", "Отключает динамические тени.", "DisableShadows", false);
                    AddToggle(panel, "Отключить пост-эффекты", "Отключает bloom и другие пост-эффекты.", "DisablePostFX", false);
                    AddSection(panel, "Дополнительно");
                    AddInfo(panel, "Редактируйте CustomFastFlags.json для продвинутых настроек.");
                    AddAction(panel, "Открыть CustomFastFlags.json", "", () =>
                    {
                        FastFlagManager.CreateExampleFile();
                        Process.Start("notepad.exe", Path.Combine(RobloxInstaller.BaseFolder, "CustomFastFlags.json"));
                    });
                    break;

                case "Appearance":
                    AddHeader(panel, "Внешний вид", "Настройте внешний вид Foxstrap.");
                    AddToggle(panel, "Скруглённые углы", "Скругляет углы элементов интерфейса.", "RoundedCorners", true);
                    AddToggle(panel, "Использовать акцентный цвет Windows", "Применяет системный акцентный цвет.", "UseAccentColor", false);
                    break;

                case "Shortcuts":
                    AddHeader(panel, "Ярлыки", "Управляйте ярлыками Foxstrap.");
                    AddToggle(panel, "Ярлык на рабочем столе", "Создать ярлык на рабочем столе.", "DesktopShortcut", true);
                    AddToggle(panel, "Ярлык в меню Пуск", "Создать ярлык в меню Пуск.", "StartMenuShortcut", true);
                    AddToggle(panel, "Быстрый запуск Roblox", "Ярлык для прямого запуска Roblox.", "LaunchShortcut", false);
                    break;

                case "FoxstrapSettings":
                    AddHeader(panel, "Foxstrap", "Настройки приложения Foxstrap.");
                    AddToggle(panel, "Автообновление Foxstrap", "Проверять обновления при запуске.", "AutoUpdate", true);
                    AddSection(panel, "Диагностика");
                    AddAction(panel, "Открыть папку логов", "", () =>
                    {
                        Directory.CreateDirectory(Logger.GetLogDirectory());
                        Process.Start("explorer.exe", Logger.GetLogDirectory());
                    });
                    AddAction(panel, "Переустановить Roblox", "Копирует Roblox из системной установки заново.", () =>
                    {
                        if (MessageBox.Show("Переустановить Roblox?", "Foxstrap",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            RobloxInstaller.Uninstall();
                            MessageBox.Show("Roblox будет переустановлен при следующем запуске.", "Foxstrap",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    });
                    break;

                case "About":
                    AddHeader(panel, "О Foxstrap", "");
                    AddInfo(panel, "Версия: 1.0.0");
                    AddInfo(panel, "Разработчик: Foxi005305");
                    AddInfo(panel, "Foxstrap — кастомный лаунчер для Roblox на базе .NET 9 и WPF.");
                    AddInfo(panel, $"Платформа: .NET {Environment.Version}");
                    break;
            }
        }

        private static Color C(byte r, byte g, byte b) => Color.FromRgb(r, g, b);

        private static void AddHeader(StackPanel p, string title, string sub)
        {
            p.Children.Add(new TextBlock { Text = title, FontSize = 24, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(C(0xCD,0xD6,0xF4)), Margin = new Thickness(0,0,0,4) });
            if (!string.IsNullOrEmpty(sub))
                p.Children.Add(new TextBlock { Text = sub, FontSize = 12, Foreground = new SolidColorBrush(C(0xA6,0xAD,0xC8)), Margin = new Thickness(0,0,0,20), TextWrapping = TextWrapping.Wrap });
        }

        private static void AddSection(StackPanel p, string text)
            => p.Children.Add(new TextBlock { Text = text, FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(C(0xCD,0xD6,0xF4)), Margin = new Thickness(0,14,0,10) });

        private static void AddInfo(StackPanel p, string text)
            => p.Children.Add(new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(C(0xA6,0xAD,0xC8)), Margin = new Thickness(0,0,0,8), TextWrapping = TextWrapping.Wrap });

        private void AddToggle(StackPanel p, string label, string desc, string key, bool def)
        {
            var border = new Border { Background = new SolidColorBrush(C(0x31,0x32,0x44)), CornerRadius = new CornerRadius(8), Padding = new Thickness(16,14,16,14), Margin = new Thickness(0,0,0,6) };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var tp = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,16,0) };
            tp.Children.Add(new TextBlock { Text = label, FontSize = 13, FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(C(0xCD,0xD6,0xF4)) });
            if (!string.IsNullOrEmpty(desc))
                tp.Children.Add(new TextBlock { Text = desc, FontSize = 11, Foreground = new SolidColorBrush(C(0xA6,0xAD,0xC8)), Margin = new Thickness(0,3,0,0), TextWrapping = TextWrapping.Wrap });
            var toggle = new CheckBox { IsChecked = FoxstrapSettings.Get(key, def), VerticalAlignment = VerticalAlignment.Center, Style = (Style)FindResource("ToggleSwitch") };
            toggle.Checked   += (s, e) => FoxstrapSettings.Set(key, true);
            toggle.Unchecked += (s, e) => FoxstrapSettings.Set(key, false);
            Grid.SetColumn(toggle, 1);
            grid.Children.Add(tp); grid.Children.Add(toggle);
            border.Child = grid; p.Children.Add(border);
        }

        private static void AddAction(StackPanel p, string label, string desc, Action onClick)
        {
            var border = new Border { Background = new SolidColorBrush(C(0x31,0x32,0x44)), CornerRadius = new CornerRadius(8), Padding = new Thickness(16,14,16,14), Margin = new Thickness(0,0,0,6), Cursor = System.Windows.Input.Cursors.Hand };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var tp = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            tp.Children.Add(new TextBlock { Text = label, FontSize = 13, FontWeight = FontWeights.Medium, Foreground = new SolidColorBrush(C(0xCD,0xD6,0xF4)) });
            if (!string.IsNullOrEmpty(desc))
                tp.Children.Add(new TextBlock { Text = desc, FontSize = 11, Foreground = new SolidColorBrush(C(0xA6,0xAD,0xC8)), Margin = new Thickness(0,3,0,0) });
            var arrow = new TextBlock { Text = "\uE76C", FontFamily = new System.Windows.Media.FontFamily("Segoe MDL2 Assets"), FontSize = 14, VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(C(0xA6,0xAD,0xC8)) };
            Grid.SetColumn(arrow, 1);
            grid.Children.Add(tp); grid.Children.Add(arrow);
            border.Child = grid;
            border.MouseLeftButtonUp += (s, e) => onClick();
            p.Children.Add(border);
        }

        private void AddFontPicker(StackPanel p)
        {
            var border = new Border { Background = new SolidColorBrush(C(0x31,0x32,0x44)), CornerRadius = new CornerRadius(8), Padding = new Thickness(16,14,16,14), Margin = new Thickness(0,0,0,6) };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            string cur = FoxstrapSettings.Get("CustomFont", "");
            var lbl = new TextBlock { Text = string.IsNullOrEmpty(cur) ? "Шрифт не выбран" : Path.GetFileName(cur), FontSize = 13, VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(C(0xA6,0xAD,0xC8)) };
            var browse = new Button { Content = "Выбрать", Margin = new Thickness(8,0,0,0), Padding = new Thickness(12,6,12,6), Background = new SolidColorBrush(C(0xCB,0xA6,0xF7)), Foreground = new SolidColorBrush(C(0x1E,0x1E,0x2E)), BorderThickness = new Thickness(0), FontWeight = FontWeights.SemiBold, Cursor = System.Windows.Input.Cursors.Hand };
            browse.Click += (s, e) =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Выберите шрифт", Filter = "Файлы шрифтов|*.ttf;*.otf" };
                if (dlg.ShowDialog() == true) { FoxstrapSettings.Set("CustomFont", dlg.FileName); lbl.Text = Path.GetFileName(dlg.FileName); }
            };
            var clear = new Button { Content = "Сбросить", Margin = new Thickness(8,0,0,0), Padding = new Thickness(12,6,12,6), Background = new SolidColorBrush(C(0x45,0x47,0x5A)), Foreground = new SolidColorBrush(C(0xCD,0xD6,0xF4)), BorderThickness = new Thickness(0), Cursor = System.Windows.Input.Cursors.Hand };
            clear.Click += (s, e) => { FoxstrapSettings.Set("CustomFont", ""); lbl.Text = "Шрифт не выбран"; };
            Grid.SetColumn(browse, 1); Grid.SetColumn(clear, 2);
            grid.Children.Add(lbl); grid.Children.Add(browse); grid.Children.Add(clear);
            border.Child = grid; p.Children.Add(border);
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e) { FoxstrapSettings.Save(); Close(); new LoadingWindow().Show(); }
        private void SaveAndLaunch_Click(object sender, RoutedEventArgs e) { FoxstrapSettings.Save(); Close(); new LoadingWindow().Show(); }
        private void SaveButton_Click(object sender, RoutedEventArgs e) { FoxstrapSettings.Save(); MessageBox.Show("Настройки сохранены!", "Foxstrap", MessageBoxButton.OK, MessageBoxImage.Information); }
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
        private void TestMode_Changed(object sender, RoutedEventArgs e)
        {
            if (FindName("TestModeCheck") is CheckBox cb)
                FoxstrapSettings.Set("TestMode", cb.IsChecked == true);
        }
    }
}
