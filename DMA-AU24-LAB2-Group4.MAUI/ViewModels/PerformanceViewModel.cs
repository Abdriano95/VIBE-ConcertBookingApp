using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    // QueryProperty does not work at all, so we have resorted to IQueryAttributable instead because this works
    public partial class PerformanceViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IRestService _restService;

        [ObservableProperty]
        private ObservableCollection<Performance> performances;

        [ObservableProperty]
        private Performance selectedPerformance;

        //The command did not work for some wierd reason, mvvmtoolkit could not find the BookPerformanceAsyncCommand command
        // so we had to manually created it in order for it to work. 
        public ICommand BookPerformanceAsyncCommand { get; }

        [ObservableProperty]
        private int _concertId;


        public PerformanceViewModel(IRestService restService)
        {
            _restService = restService;
            BookPerformanceAsyncCommand = new Command<Performance>(async (performance) => await BookPerformanceAsync(performance));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ConcertId", out var concertIdValue))
            {
                ConcertId = int.Parse(concertIdValue.ToString());
                Debug.WriteLine($"ConcertId received: {ConcertId}");
                LoadAvailablePerformancesCommand.Execute(null);
            }
            else
            {
                Debug.WriteLine("No ConcertId found in query attributes.");
            }
        }

        [RelayCommand]
        public async Task LoadAvailablePerformancesAsync()
        {
            Debug.WriteLine($"Reloading performances for ConcertId: {ConcertId}");
            int customerId = Preferences.Get("CustomerId", 0);
            Debug.WriteLine($"Fetching available performances for ConcertId: {ConcertId} and CustomerId: {customerId}");

            try
            {
                var performances = await _restService.GetAvailablePerformancesAsync(ConcertId, customerId);
                Performances = performances ?? new ObservableCollection<Performance>();
                Debug.WriteLine($"ConcertId received: {ConcertId}");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading performances: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load performances.", "OK");
            }
        }

        [RelayCommand]
        public async Task BookPerformanceAsync(Performance performance)
        {
            if (performance == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No performance selected.", "OK");
                Debug.WriteLine("Performance is null.");
                return;
            }

            int customerId = Preferences.Get("CustomerId", 0);

            if (customerId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No customer ID found. Please log in.", "OK");
                Debug.WriteLine("CustomerId is 0.");
                return;
            }

            // Bekräfta att användaren vill boka
            var confirm = await Application.Current.MainPage.DisplayAlert("Confirm",
                $"Do you want to book this performance at {performance.Venue} on {performance.PerformanceDateAndTime}?",
                "Yes", "No");

            if (!confirm)
            {
                Debug.WriteLine("User cancelled booking.");
                return;
            }

            Debug.WriteLine($"CustomerId: {customerId}, PerformanceId: {performance.Id}");

            var bookingDto = new BookingCreateDto
            {
                CustomerId = customerId,
                PerformanceId = performance.Id
            };

            // Skapa bokning via RestService
            bool success = await _restService.CreateBookingAsync(bookingDto);

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Booking created successfully.", "OK");
                // Uppdatera performances för att reflektera den nya bokningen
                await LoadAvailablePerformancesAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to create booking.", "OK");
            }
        }

    }
}
