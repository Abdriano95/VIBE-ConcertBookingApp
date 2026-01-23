using CommunityToolkit.Maui.Views;
using DMA_AU24_LAB2_Group4.MAUI.ViewModels;
using Microsoft.Extensions.Logging;

namespace DMA_AU24_LAB2_Group4.MAUI.Views;

public partial class LoginPage : ContentPage
{
    private readonly ILogger<LoginPage> _logger;
    private bool _videoInitialized = false;

    public LoginPage(LoginViewModel viewModel, ILogger<LoginPage> logger)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _logger = logger;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Start the background video
        if (BackgroundVideo != null && !_videoInitialized)
        {
            try
            {
                BackgroundVideo.Source = MediaSource.FromResource("concertvideo.mp4");
                _videoInitialized = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize background video");
            }
        }
        
        // Resume if already initialized
        if (_videoInitialized)
        {
            BackgroundVideo?.Play();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Stop and clean up video when leaving page
        if (BackgroundVideo != null)
        {
            BackgroundVideo.Stop();
            BackgroundVideo.Handler?.DisconnectHandler();
        }
    }
}