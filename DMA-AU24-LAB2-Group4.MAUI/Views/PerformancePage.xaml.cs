using DMA_AU24_LAB2_Group4.MAUI.Models;
using DMA_AU24_LAB2_Group4.MAUI.ViewModels;

namespace DMA_AU24_LAB2_Group4.MAUI.Views;

public partial class PerformancePage : ContentPage
{
    private readonly PerformanceViewModel _viewModel;

    public PerformancePage(PerformanceViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// Handles the Book Now button click - calls the ViewModel's BookPerformance method.
    /// </summary>
    private async void OnBookNowClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Performance performance)
        {
            await _viewModel.BookPerformance(performance);
        }
    }
}