using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ICustomerService _customerService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [RelayCommand]
        public async Task Login()
        {
            var customer = await _customerService.LoginAsync(Email, Password);
            if (customer == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid email or password", "OK");
                return;
            }

            // Navigera till huvudvyn efter lyckad inloggning
            await Shell.Current.GoToAsync("//ConcertsPage");
        }

        [RelayCommand]
        public async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}
