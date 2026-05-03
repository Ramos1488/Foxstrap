using System.Windows;

namespace Foxstrap.UI.Windows
{
    public partial class LoadingWindow : Window
    {
        private bool _cancelled = false;
        public bool Cancelled => _cancelled;

        public LoadingWindow(string status = "Запускаем Roblox...")
        {
            InitializeComponent();
            StatusText.Text = status;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancelled = true;
            Close();
        }
    }
}

