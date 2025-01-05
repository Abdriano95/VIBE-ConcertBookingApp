using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DMA_AU24_LAB2_Group4.MAUI.ViewModels
{
    [ObservableObject]
    public partial class PerformanceViewModel
    {
        [ObservableProperty]
        private string performanceDetails;

        public PerformanceViewModel()
        {
            // Default state
            PerformanceDetails = "Loading performance details...";
        }

        [RelayCommand]
        public async Task LoadPerformanceDetails(string concertId)
        {
            if (string.IsNullOrEmpty(concertId)) return;

            // Simulate loading data
            await Task.Delay(500); // Replace with actual API or service call
            PerformanceDetails = $"Details for concert ID: {concertId}";
        }
    }
}
