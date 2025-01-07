using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;


namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class ProfileViewModel : ObservableValidator
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        private string firstName;

        [ObservableProperty]
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        private string lastName;

        [ObservableProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        private string email;

        [ObservableProperty]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(60, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 60 characters.")]
        private string password;

        public ProfileViewModel(IRestService restService)
        {
            _restService = restService;
            LoadProfileCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadProfile()
        {
            int customerId = Preferences.Get("CustomerId", 0);
            Debug.WriteLine($"CustomerId from Preferences: {customerId}");

            if (customerId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No customer ID found. Please log in.", "OK");
                return;
            }

            var profile = await _restService.GetProfileAsync(customerId);
            if (profile != null)
            {
                FirstName = profile.FirstName;
                LastName = profile.LastName;
                Email = profile.Email;
                Password = profile.Password;

                Debug.WriteLine($"Loaded Customer: {FirstName}, {LastName}, {Email}");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load profile.", "OK");
            }
        }





        [RelayCommand]
        public async Task UpdateProfile()
        {
            ValidateAllProperties();

            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors(null).Select(e => e.ErrorMessage));
                await Application.Current.MainPage.DisplayAlert("Validation Error", errors, "OK");
                return;
            }

            var customer = new Customer
            {
                Id = Preferences.Get("CustomerId", 0),
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };

            var success = await _restService.UpdateProfileAsync(customer);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Profile updated successfully.", "OK");
                LoadProfileCommand.Execute(null);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update profile.", "OK");
                LoadProfileCommand.Execute(null);

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

            Debug.WriteLine($"IsLoggedIn after clear: {Preferences.ContainsKey("IsLoggedIn")}");

            // Navigate to login page after successful logout
            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
