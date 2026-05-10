namespace Foxstrap.Windows
{
    public partial class AdminWindow : Window
    {
        public AdminWindow() => InitializeComponent();

        private void Titlebar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void Input_GotFocus(object sender, RoutedEventArgs e) { }
        private void Input_LostFocus(object sender, RoutedEventArgs e) { }
        private void AddUser_Click(object sender, RoutedEventArgs e) { }
        private void Export_Click(object sender, RoutedEventArgs e) { }
    }
}

