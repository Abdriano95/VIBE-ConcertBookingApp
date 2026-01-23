using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service interface for booking-related API operations.
    /// </summary>
    public interface IApiBookingService
    {
        /// <summary>
        /// Gets all bookings from the API.
        /// </summary>
        Task<ObservableCollection<Booking>?> GetAllBookingsAsync();

        /// <summary>
        /// Gets a specific booking by ID.
        /// </summary>
        Task<Booking?> GetBookingByIdAsync(int bookingId);

        /// <summary>
        /// Gets all bookings for a specific customer.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByCustomerIdAsync(int customerId);

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        Task<bool> CreateBookingAsync(BookingCreateDto bookingDto);

        /// <summary>
        /// Saves (creates or updates) a booking.
        /// </summary>
        Task SaveBookingAsync(Booking booking, bool isNewBooking);

        /// <summary>
        /// Deletes a booking by ID.
        /// </summary>
        Task<bool> DeleteBookingAsync(int bookingId);
    }
}
