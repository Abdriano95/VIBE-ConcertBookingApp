using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;


namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class MyBookingsViewModel : ObservableObject
    {
        private readonly IApiBookingService _bookingService;
        private readonly ILogger<MyBookingsViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<Booking> bookings = new();

        [ObservableProperty]
        private Booking? selectedBooking;

        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Called when SelectedBooking changes - navigates to booking details page.
        /// </summary>
        partial void OnSelectedBookingChanged(Booking? value)
        {
            if (value != null)
            {
                // Fire-and-forget navigation, then clear selection
                _ = NavigateToBookingDetailsAsync(value);
            }
        }

        private async Task NavigateToBookingDetailsAsync(Booking booking)
        {
            try
            {
                _logger.LogDebug("Navigating to BookingDetailsPage for BookingId {BookingId}", booking.BookingId);
                await Shell.Current.GoToAsync($"BookingDetailsPage?bookingId={booking.BookingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to BookingDetailsPage for BookingId {BookingId}", booking.BookingId);
                await Shell.Current.DisplayAlert("Error", "Failed to navigate to booking details.", "OK");
            }
            finally
            {
                // Clear selection so the same item can be selected again
                SelectedBooking = null;
            }
        }

        public MyBookingsViewModel(IApiBookingService bookingService, ILogger<MyBookingsViewModel> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
            // Fire-and-forget initial load; Appearing event handles reloads
            _ = LoadBookings();
        }

        [RelayCommand]
        public async Task LoadBookings()
        {
            try
            {
                IsBusy = true;

                int customerId = Preferences.Get("CustomerId", 0);
                if (customerId == 0)
                {
                    await Shell.Current.DisplayAlert("Error", "No customer ID found.", "OK");
                    return;
                }

                var bookings = await _bookingService.GetBookingsByCustomerIdAsync(customerId);
                if (bookings != null)
                {
                    Bookings = new ObservableCollection<Booking>(bookings);
                }
                else
                {
                    Bookings = new ObservableCollection<Booking>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task NavigateToBookingDetails(Booking selectedBooking)
        {
            if (selectedBooking != null)
            {
                _logger.LogDebug("Navigating to BookingDetailsPage for BookingId {BookingId}", selectedBooking.BookingId);
                await Shell.Current.GoToAsync($"BookingDetailsPage?bookingId={selectedBooking.BookingId}");
            }
        }
    }
}
