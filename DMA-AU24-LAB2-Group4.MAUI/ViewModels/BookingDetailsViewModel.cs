using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using Microsoft.Extensions.Logging;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class BookingDetailsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IApiBookingService _bookingService;
        private readonly ILogger<BookingDetailsViewModel> _logger;

        [ObservableProperty]
        private Booking? booking;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isDeleting;

        public BookingDetailsViewModel(IApiBookingService bookingService, ILogger<BookingDetailsViewModel> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("bookingId", out var bookingId) && bookingId != null)
            {
                _logger.LogDebug("Received BookingId: {BookingId}", bookingId);
                if (int.TryParse(bookingId.ToString(), out int parsedBookingId))
                {
                    LoadBookingDetailsCommand.Execute(parsedBookingId);
                }
                else
                {
                    _logger.LogWarning("Failed to parse BookingId: {BookingId}", bookingId);
                }
            }
            else
            {
                _logger.LogWarning("No BookingId found in query attributes");
            }
        }

        [RelayCommand]
        public async Task LoadBookingDetails(int bookingId)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                _logger.LogDebug("Loading booking details for BookingId {BookingId}", bookingId);
                var bookingDetails = await _bookingService.GetBookingByIdAsync(bookingId);

                if (bookingDetails != null)
                {
                    Booking = bookingDetails;
                    _logger.LogDebug("Loaded booking: {ConcertTitle} at {Venue} on {PerformanceDate}", 
                        Booking.ConcertTitle, Booking.Venue, Booking.PerformanceDate);
                }
                else
                {
                    _logger.LogWarning("Failed to load booking details for BookingId {BookingId}", bookingId);
                    await Shell.Current.DisplayAlert("Error", "Failed to load booking details.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        public async Task DeleteBooking()
        {
            if (IsDeleting) return;

            var confirm = await Shell.Current.DisplayAlert("Confirm", "Do you really want to delete this booking?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    IsDeleting = true;

                    if (Booking == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "No booking to delete.", "OK");
                        return;
                    }
                    
                    var success = await _bookingService.DeleteBookingAsync(Booking.BookingId);
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", "Booking deleted successfully.", "OK");
                        await Shell.Current.GoToAsync(".."); // Navigate back to bookings list
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to delete booking.", "OK");
                    }
                }
                finally
                {
                    IsDeleting = false;
                }
            }
        }
    }
}
