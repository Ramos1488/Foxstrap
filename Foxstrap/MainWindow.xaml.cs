using Foxstrap.ViewModels;
using System.Windows;

namespace Foxstrap
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ServiceLocator.Get<MainViewModel>();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.LaunchCommand.CanExecute(null))
            {
                vm.LaunchCommand.Execute(null);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}