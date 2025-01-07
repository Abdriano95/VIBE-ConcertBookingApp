using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System.Diagnostics;


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
            if (customer != null)
            {
                //Clear prefrences before saving new customer ID
                Preferences.Clear();


                // Updates the customer ID in the preferences and sets the IsLoggedIn flag to true
                Preferences.Set("CustomerId", customer.Id);
                Preferences.Set("IsLoggedIn", true);

                Debug.WriteLine($"CustomerId saved in Preferences: {customer.Id}");

                // Navigera till profilsidan
                await Shell.Current.GoToAsync("//ConcertsPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid email or password.", "OK");
            }
        }


        [RelayCommand]
        public async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}
