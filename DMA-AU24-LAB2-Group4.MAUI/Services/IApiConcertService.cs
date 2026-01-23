using DMA_AU24_LAB2_Group4.MAUI.Models;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service interface for concert-related API operations.
    /// </summary>
    public interface IApiConcertService
    {
        /// <summary>
        /// Gets all concerts from the API.
        /// </summary>
        Task<ObservableCollection<Concert>?> GetAllConcertsAsync();

        /// <summary>
        /// Gets a specific concert by ID.
        /// </summary>
        Task<Concert?> GetConcertByIdAsync(int concertId);
    }
}
