using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using DMA_AU24_LAB2_Group4.MAUI.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    public partial class ConcertViewModel : ObservableObject
    {
        private readonly IApiConcertService _concertService;
        private readonly ILogger<ConcertViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<Concert> concertItems = new();

        [ObservableProperty]
        private Concert? selectedConcert;

        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Called when SelectedConcert changes - navigates to performances page.
        /// </summary>
        partial void OnSelectedConcertChanged(Concert? value)
        {
            if (value != null)
            {
                // Fire-and-forget navigation, then clear selection
                _ = NavigateToPerformancesAsync(value);
            }
        }

        private async Task NavigateToPerformancesAsync(Concert concert)
        {
            try
            {
                _logger.LogDebug("Navigating to performances for ConcertId {ConcertId}", concert.Id);
                await Shell.Current.GoToAsync($"PerformancePage?ConcertId={concert.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to PerformancePage for ConcertId {ConcertId}", concert.Id);
                await Shell.Current.DisplayAlert("Error", "Failed to navigate to performances.", "OK");
            }
            finally
            {
                // Clear selection so the same item can be selected again
                SelectedConcert = null;
            }
        }

        public ConcertViewModel(IApiConcertService concertService, ILogger<ConcertViewModel> logger)
        {
            _concertService = concertService;
            _logger = logger;
            // Fire-and-forget initial load; Appearing event handles reloads
            _ = LoadConcertsAsync();
        }

        [RelayCommand]
        public async Task LoadConcertsAsync()
        {
            try
            {
                IsBusy = true;

                // Fetch all concerts
                var concerts = await _concertService.GetAllConcertsAsync();
                ConcertItems = concerts ?? new ObservableCollection<Concert>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading concerts");
                await Shell.Current.DisplayAlert("Error", "Failed to load concerts.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ShowPerformances(Concert concert)
        {
            if (concert == null)
            {
                _logger.LogWarning("ShowPerformances called with null concert");
                return;
            }

            try
            {
                _logger.LogDebug("Navigating to performances for ConcertId {ConcertId}", concert.Id);
                await Shell.Current.GoToAsync($"PerformancePage?ConcertId={concert.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to PerformancePage for ConcertId {ConcertId}", concert.Id);
                await Shell.Current.DisplayAlert("Error", "Failed to navigate to performances.", "OK");
            }
        }
    }
}
