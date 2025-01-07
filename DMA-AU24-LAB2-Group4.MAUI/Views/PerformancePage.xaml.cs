using DMA_AU24_LAB2_Group4.MAUI.ViewModels;

namespace DMA_AU24_LAB2_Group4.MAUI.Views;

public partial class PerformancePage : ContentPage
{
    private PerformanceViewModel _viewModel;
    public PerformancePage(PerformanceViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAvailablePerformancesAsync();
    }
}