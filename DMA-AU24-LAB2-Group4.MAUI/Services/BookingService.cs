using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public class BookingService : IBookingService
    {
        IRestService _restService;
        public BookingService(IRestService service)
        {
            _restService = service;
        }
        public Task<ObservableCollection<Booking>?> GetBookingAsync()
        {
            return _restService.RefreshDataAsync();
        }
        public Task SaveBookingAsync(Booking booking, bool isNewBooking = false)
        {
            return _restService.SaveBookingAsync(booking, isNewBooking);
        }
        public Task DeleteBookingAsync(Booking booking)
        {
            return _restService.DeleteBookingAsync(booking.Id);
        }
    }
}