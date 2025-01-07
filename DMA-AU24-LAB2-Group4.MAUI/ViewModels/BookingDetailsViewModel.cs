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
    public partial class BookingDetailsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private Booking booking;

        public BookingDetailsViewModel(IRestService restService)
        {
            _restService = restService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("bookingId", out var bookingId))
            {
                Debug.WriteLine($"Received BookingId: {bookingId}");
                LoadBookingDetailsCommand.Execute(int.Parse(bookingId.ToString()));
            }
            else
            {
                Debug.WriteLine("No BookingId found in query attributes.");
            }
        }

        [RelayCommand]
        public async Task LoadBookingDetails(int bookingId)
        {
            Debug.WriteLine($"Loading booking details for BookingId: {bookingId}");
            var bookingDetails = await _restService.GetBookingByIdAsync(bookingId);

            if (bookingDetails != null)
            {
                Booking = bookingDetails;
                Debug.WriteLine($"Loaded Booking: {Booking.ConcertTitle}, {Booking.PerformanceDate}, {Booking.Venue}");
            }
            else
            {
                Debug.WriteLine("Failed to load booking details from RestService.");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load booking details.", "OK");
            }
        }


        [RelayCommand]
        public async Task DeleteBooking()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert("Confirm", "Do you really want to delete this booking?", "Yes", "No");
            if (confirm)
            {
                var success = await _restService.DeleteBookingAsync(Booking.BookingId);
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Booking deleted successfully.", "OK");
                    await Shell.Current.GoToAsync(".."); // Navigera tillbaka till bokningslistan
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete booking.", "OK");
                }
            }
        }
    }
}
