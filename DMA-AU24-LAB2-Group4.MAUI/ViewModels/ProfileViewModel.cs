using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;


namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class ProfileViewModel : ObservableValidator
    {
        private readonly IApiCustomerService _customerService;
        private readonly ILogger<ProfileViewModel> _logger;

        [ObservableProperty]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        private string firstName = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        private string lastName = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        private string email = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(60, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 60 characters.")]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isSaving;

        public ProfileViewModel(IApiCustomerService customerService, ILogger<ProfileViewModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
            LoadProfileCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadProfile()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                int customerId = Preferences.Get("CustomerId", 0);
                _logger.LogDebug("Loading profile for CustomerId {CustomerId}", customerId);

                if (customerId == 0)
                {
                    await Shell.Current.DisplayAlert("Error", "No customer ID found. Please log in.", "OK");
                    return;
                }

                var profile = await _customerService.GetProfileAsync(customerId);
                if (profile != null)
                {
                    FirstName = profile.FirstName;
                    LastName = profile.LastName;
                    Email = profile.Email;
                    Password = profile.Password;

                    _logger.LogDebug("Loaded profile for {FirstName} {LastName}", FirstName, LastName);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to load profile.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task UpdateProfile()
        {
            if (IsSaving) return;

            ValidateAllProperties();

            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors(null).Select(e => e.ErrorMessage));
                await Shell.Current.DisplayAlert("Validation Error", errors, "OK");
                return;
            }

            try
            {
                IsSaving = true;

                var customer = new Customer
                {
                    Id = Preferences.Get("CustomerId", 0),
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password
                };

                var success = await _customerService.UpdateProfileAsync(customer);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Profile updated successfully.", "OK");
                    await LoadProfile();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to update profile.", "OK");
                }
            }
            finally
            {
                IsSaving = false;
            }
        }


        [RelayCommand]
        public async Task Logout()
        {
            // Empty the fields
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;

            // Clear the Preferences and set IsLoggedIn to false
            Preferences.Clear();
            Preferences.Set("IsLoggedIn", false);

            _logger.LogInformation("User logged out successfully");

            // Navigate to login page after successful logout
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
