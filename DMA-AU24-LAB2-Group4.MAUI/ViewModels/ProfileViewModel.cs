using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {




        [RelayCommand]
        public async Task Logout()
        {
            Preferences.Clear(); // Delete all preferences
            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
