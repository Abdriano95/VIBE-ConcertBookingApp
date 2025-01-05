using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private Customer customer;

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
                await Application.Current.MainPage.DisplayAlert("Error", "No customer ID found.", "OK");
                return;
            }

            var profile = await _restService.GetProfileAsync(customerId);
            if (profile != null)
            {
                Customer = profile; // Binda data till Customer-objekt
                Debug.WriteLine($"Loaded Customer: {Customer.FirstName}, {Customer.LastName}, {Customer.Email}, {Customer.Password}");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load profile.", "OK");
            }
        }



        [RelayCommand]
        public async Task UpdateProfile()
        {
            var success = await _restService.UpdateProfileAsync(Customer);
            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Profile updated successfully.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update profile.", "OK");
            }
        }

        [RelayCommand]
        public async Task Logout()
        {
            Preferences.Clear(); // Delete all preferences
            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
