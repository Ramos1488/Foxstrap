﻿using Foxstrap.Commands;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Foxstrap.Services;
using System.Windows.Input;

namespace Foxstrap.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _status = "Р“РѕС‚РѕРІ";
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
                Status = "Р—Р°РїСѓСЃРє Roblox...";
                Logger.Info("Launching Roblox");

                // TODO: С‚РІРѕСЏ Р»РѕРіРёРєР° Р·Р°РїСѓСЃРєР°
                await Task.Delay(500);

                Status = "Roblox Р·Р°РїСѓС‰РµРЅ";
                Logger.Info("Roblox launched successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to launch Roblox" + " " + ex);
                Status = "РћС€РёР±РєР° Р·Р°РїСѓСЃРєР°";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}



