using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    [QueryProperty(nameof(ConcertId), "ConcertId")]
    public partial class PerformanceViewModel : ObservableObject
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private ObservableCollection<Performance> performances;

        [ObservableProperty]
        private int concertId;

        public PerformanceViewModel(IRestService restService)
        {
            _restService = restService;

            // Load available performances
        }

        [RelayCommand]
        public async Task LoadAvailablePerformancesAsync()
        {
            int customerId = Preferences.Get("CustomerId", 0);

            if (customerId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No customer ID found. Please log in.", "OK");
                return;
            }

            Performances = await _restService.GetAvailablePerformancesAsync(ConcertId, customerId);
        }
    }
}
