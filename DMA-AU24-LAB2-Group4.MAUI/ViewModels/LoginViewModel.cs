using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using Microsoft.Extensions.Logging;


namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IApiCustomerService _customerService;
        private readonly ILogger<LoginViewModel> _logger;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        public LoginViewModel(IApiCustomerService customerService, ILogger<LoginViewModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [RelayCommand]
        public async Task Login()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var customer = await _customerService.LoginAsync(Email, Password);
                if (customer != null)
                {
                    //Clear prefrences before saving new customer ID
                    Preferences.Clear();

                    // Updates the customer ID in the preferences and sets the IsLoggedIn flag to true
                    Preferences.Set("CustomerId", customer.Id);
                    Preferences.Set("IsLoggedIn", true);

                    _logger.LogInformation("User logged in successfully. CustomerId: {CustomerId}", customer.Id);

                    // Navigate to concerts page
                    await Shell.Current.GoToAsync("//ConcertsPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid email or password.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        public async Task NavigateToRegister()
        {
            // Use absolute navigation to avoid page stacking
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
