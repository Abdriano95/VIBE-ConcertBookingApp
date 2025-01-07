using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public interface IRestService
    {
        // Booking
        Task<ObservableCollection<Booking>?> RefreshDataAsync();
        Task SaveBookingAsync(Booking booking, bool isNewBooking);
        Task<bool> CreateBookingAsync(BookingCreateDto bookingDto);

        Task<IEnumerable<Booking>> GetBookingsByCustomerIdAsync(int customerId);
        Task<bool> DeleteBookingAsync(int bookingId);
        Task<Booking?> GetBookingByIdAsync(int bookingId);

        // Performances
        Task<ObservableCollection<Performance>> GetAvailablePerformancesAsync(int concertId, int customerId);


        // Customer
        Task<bool> RegisterCustomerAsync(Customer customer);
        Task<Customer?> LoginAsync(string email, string password);
        Task<Customer?> GetProfileAsync(int customerId);
        Task<bool> UpdateProfileAsync(Customer customer);

        // Concert
        Task<ObservableCollection<Concert>?> RefreshConcertDataAsync();
        Task<Concert?> GetConcertByIdAsync(int id);
    }
}