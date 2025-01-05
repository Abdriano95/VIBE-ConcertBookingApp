using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
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
            ConcertItems = new(await _restService.RefreshConcertDataAsync() ?? new ObservableCollection<Concert>());
        }

        [RelayCommand]
        public async Task SelectionChanged()
        {
            if (SelectedConcert == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { nameof(Concert), selectedConcert }
            };
            await Shell.Current.GoToAsync("PerformancePage", navigationParameter);
        }
    }
}
