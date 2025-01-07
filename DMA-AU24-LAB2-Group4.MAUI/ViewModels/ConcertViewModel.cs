using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using DMA_AU24_LAB2_Group4.MAUI.Views;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    [ObservableObject]
    public partial class ConcertViewModel
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private ObservableCollection<Concert> concertItems = new();

        [ObservableProperty]
        private Concert? selectedConcert;

        public ConcertViewModel(IRestService restService)
        {
            _restService = restService;
        }

        [RelayCommand]
        public async Task Appearing()
        {
             var concerts = await _restService.RefreshConcertDataAsync(); // Ensure you're calling RefreshConcertDataAsync
            ConcertItems = concerts ?? new ObservableCollection<Concert>(); // Set ConcertItems

            // concertItems = new(await _restService.RefreshConcertDataAsync() ?? new ObservableCollection<Concert>());

        }

        [RelayCommand]
        public async Task SelectionChanged(Concert selectedConcert)
        {
            if (selectedConcert == null) return;

            var concertId = selectedConcert.Id;
            var navigationParameter = new Dictionary<string, object>
            {
                { nameof(Concert), selectedConcert }
            };
            //await Shell.Current.GoToAsync("PerformancePage", navigationParameter);
            //await Shell.Current.GoToAsync($"{nameof(PerformancePage)}?concertId={concertId}");
            await Shell.Current.GoToAsync($"///PerformancePage", navigationParameter);
            SelectedConcert = null;
        }

        [RelayCommand]
        public async Task ShowPerformances(Concert concert)
        {
            if (concert == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(PerformancePage)}?ConcertId={concert.Id}");
        }

    }
}
