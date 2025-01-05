using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class RegisterViewModel : ObservableValidator
    {
        private readonly ICustomerService _customerService;

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

        [ObservableProperty]
        [Required(ErrorMessage = "Confirm Password is required.")]
        private string confirmPassword;

        public RegisterViewModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [RelayCommand]
        public async Task Register()
        {

            //Validate the model
            ValidateAllProperties();

            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors(null).Select(e => e.ErrorMessage));
                await Application.Current.MainPage.DisplayAlert("Validation Error", errors, "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Passwords do not match", "OK");
                return;
            }


            var customer = new Customer
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword
            };

            var success = await _customerService.RegisterCustomerAsync(customer);
            if (!success)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Registration failed", "OK");
                return;
            }

            // Clear the fields
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;


            // Navigate to login page after successful registration
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

