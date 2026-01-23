using DMA_AU24_LAB2_Group4.MAUI.Models;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service interface for performance-related API operations.
    /// </summary>
    public interface IApiPerformanceService
    {
        /// <summary>
        /// Gets available performances for a concert that the customer hasn't already booked.
        /// </summary>
        Task<ObservableCollection<Performance>> GetAvailablePerformancesAsync(int concertId, int customerId);
    }
}
