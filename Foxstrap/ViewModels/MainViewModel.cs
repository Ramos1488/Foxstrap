using Foxstrap.Commands;
using Foxstrap.Services;
using System.Windows.Input;

namespace Foxstrap.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _status = "Готов";
        private bool _isBusy = false;

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        public ICommand LaunchCommand { get; }

        public MainViewModel()
        {
            LaunchCommand = new RelayCommand(
                execute: async () => await LaunchRobloxAsync(),
                canExecute: () => IsNotBusy
            );
        }

        private async Task LaunchRobloxAsync()
        {
            try
            {
                IsBusy = true;
                Status = "Запуск Roblox...";
                Logger.Info("Launching Roblox");

                // TODO: твоя логика запуска
                await Task.Delay(500);

                Status = "Roblox запущен";
                Logger.Info("Roblox launched successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to launch Roblox", ex);
                Status = "Ошибка запуска";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}