using DMA_AU24_LAB2_Group4.MAUI.Models;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public interface IConcertService
    {
        Task<ObservableCollection<Concert>?> GetConcertsAsync();
        Task<Concert?> GetConcertByIdAsync(int id);
    }
}
