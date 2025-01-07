using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class MyBookingsViewModel : ObservableObject
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private ObservableCollection<Booking> bookings;

        public MyBookingsViewModel(IRestService restService)
        {
            _restService = restService;
            LoadBookingsCommand.Execute(null);

        }

        [RelayCommand]
        public async Task LoadBookings()
        {
            int customerId = Preferences.Get("CustomerId", 0);
            if (customerId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No customer ID found.", "OK");
                return;
            }
            var bookings = await _restService.GetBookingsByCustomerIdAsync(customerId);
            if (bookings != null)
            {
                Bookings = new ObservableCollection<Booking>(bookings);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("No Bookings", "You have no bookings.", "OK");
            }
        }

        [RelayCommand]
        public async Task NavigateToBookingDetails(Booking selectedBooking)
        {
            if (selectedBooking != null)
            {
                Debug.WriteLine($"Navigating to BookingDetailsPage with BookingId: {selectedBooking.BookingId}");
                await Shell.Current.GoToAsync($"BookingDetailsPage?bookingId={selectedBooking.BookingId}");
            }
        }

    }
}
