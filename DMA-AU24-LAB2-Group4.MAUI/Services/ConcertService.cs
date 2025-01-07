using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public partial class ConcertService : IConcertService
    {
        IRestService _restService;

        public ConcertService(IRestService service)
        {
            _restService = service;
        }

        public Task<ObservableCollection<Concert>?> GetConcertsAsync()
        {
            return _restService.RefreshConcertDataAsync();
        }

        public Task<Concert?> GetConcertByIdAsync(int id)
        {
            return _restService.GetConcertByIdAsync(id);
        }

        [RelayCommand]
        public async Task ShowPerformances(Concert concert)
        {
        //    if (concert == null)
        //        return;

        //    await Shell.Current.GoToAsync($"{nameof(PerformancePage)}?ConcertId={concert.Id}");
            throw new NotImplementedException();
        }
    }
}
