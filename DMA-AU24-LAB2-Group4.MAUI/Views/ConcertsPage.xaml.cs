using DMA_AU24_LAB2_Group4.MAUI.ViewModels;

namespace DMA_AU24_LAB2_Group4.MAUI.Views;

public partial class ConcertsPage : ContentPage
{
	public ConcertsPage(ConcertViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}