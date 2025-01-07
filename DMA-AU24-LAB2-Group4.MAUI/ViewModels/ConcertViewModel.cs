using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using DMA_AU24_LAB2_Group4.MAUI.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class ConcertViewModel : ObservableObject
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private ObservableCollection<Concert> concertItems;

        [ObservableProperty]
        private Concert selectedConcert;

        public ConcertViewModel(IRestService restService)
        {
            _restService = restService;
            LoadConcertsCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadConcertsAsync()
        {
            try
            {
                // Fetch all concerts
                var concerts = await _restService.RefreshConcertDataAsync();
                ConcertItems = concerts ?? new ObservableCollection<Concert>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading concerts: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load concerts.", "OK");
            }
        }

        [RelayCommand]
        public async Task ShowPerformances(Concert concert)
        {
            if (concert == null)
            {
                Debug.WriteLine("Concert is null.");
                return;
            }

            try
            {
                Debug.WriteLine($"Navigating to performances for ConcertId: {concert.Id}");
                await Shell.Current.GoToAsync($"PerformancePage?ConcertId={concert.Id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to PerformancePage: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to navigate to performances.", "OK");
            }
        }


    }
}
