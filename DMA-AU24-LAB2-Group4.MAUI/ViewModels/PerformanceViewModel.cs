using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    // QueryProperty does not work at all, so we have resorted to IQueryAttributable instead because this works
    public partial class PerformanceViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IApiPerformanceService _performanceService;
        private readonly IApiBookingService _bookingService;
        private readonly ILogger<PerformanceViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<Performance> performances = new();

        [ObservableProperty]
        private Performance? selectedPerformance;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isBooking;

        [ObservableProperty]
        private int _concertId;

        public PerformanceViewModel(IApiPerformanceService performanceService, IApiBookingService bookingService, ILogger<PerformanceViewModel> logger)
        {
            _performanceService = performanceService;
            _bookingService = bookingService;
            _logger = logger;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ConcertId", out var concertIdValue) && concertIdValue != null)
            {
                if (int.TryParse(concertIdValue.ToString(), out int parsedConcertId))
                {
                    ConcertId = parsedConcertId;
                    _logger.LogDebug("ConcertId received: {ConcertId}", ConcertId);
                    LoadAvailablePerformancesCommand.Execute(null);
                }
                else
                {
                    _logger.LogWarning("Failed to parse ConcertId: {ConcertIdValue}", concertIdValue);
                }
            }
            else
            {
                _logger.LogWarning("No ConcertId found in query attributes");
            }
        }

        [RelayCommand]
        public async Task LoadAvailablePerformancesAsync()
        {
            try
            {
                IsBusy = true;
                int customerId = Preferences.Get("CustomerId", 0);
                _logger.LogDebug("Loading available performances for ConcertId {ConcertId}, CustomerId {CustomerId}", 
                    ConcertId, customerId);

                var performances = await _performanceService.GetAvailablePerformancesAsync(ConcertId, customerId);
                Performances = performances ?? new ObservableCollection<Performance>();
                _logger.LogDebug("Loaded {Count} performances for ConcertId {ConcertId}", 
                    Performances.Count, ConcertId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading performances for ConcertId {ConcertId}", ConcertId);
                await Shell.Current.DisplayAlert("Error", "Failed to load performances.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task BookPerformance(Performance performance)
        {
            if (IsBooking) return;

            if (performance == null)
            {
                await Shell.Current.DisplayAlert("Error", "No performance selected.", "OK");
                _logger.LogWarning("BookPerformanceAsync called with null performance");
                return;
            }

            int customerId = Preferences.Get("CustomerId", 0);

            if (customerId == 0)
            {
                await Shell.Current.DisplayAlert("Error", "No customer ID found. Please log in.", "OK");
                _logger.LogWarning("BookPerformanceAsync called with CustomerId 0 - user not logged in");
                return;
            }

            // Confirm that the user wants to book
            var confirm = await Shell.Current.DisplayAlert("Confirm",
                $"Do you want to book this performance at {performance.Venue} on {performance.PerformanceDateAndTime}?",
                "Yes", "No");

            if (!confirm)
            {
                _logger.LogDebug("User cancelled booking for PerformanceId {PerformanceId}", performance.Id);
                return;
            }

            try
            {
                IsBooking = true;
                _logger.LogInformation("Creating booking for CustomerId {CustomerId}, PerformanceId {PerformanceId}", 
                    customerId, performance.Id);

                var bookingDto = new BookingCreateDto
                {
                    CustomerId = customerId,
                    PerformanceId = performance.Id
                };

                // Create booking via BookingService
                bool success = await _bookingService.CreateBookingAsync(bookingDto);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Booking created successfully.", "OK");
                    // Reload performances to reflect the new booking
                    await LoadAvailablePerformancesAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to create booking.", "OK");
                }
            }
            finally
            {
                IsBooking = false;
            }
        }
    }
}
