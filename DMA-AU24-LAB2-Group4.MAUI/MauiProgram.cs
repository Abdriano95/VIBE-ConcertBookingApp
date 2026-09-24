using CommunityToolkit.Maui;
using DMA_AU24_LAB2_Group4.MAUI.Services;
using Microsoft.Extensions.Logging;
using DMA_AU24_LAB2_Group4.MAUI.ViewModels;
using DMA_AU24_LAB2_Group4.MAUI.Views;
using DMA_AU24_LAB2_Group4.MAUI.Profiles;

namespace DMA_AU24_LAB2_Group4.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Services - Domain-specific API services
            builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
            builder.Services.AddSingleton<IApiBookingService, ApiBookingService>();
            builder.Services.AddSingleton<IApiConcertService, ApiConcertService>();
            builder.Services.AddSingleton<IApiCustomerService, ApiCustomerService>();
            builder.Services.AddSingleton<IApiPerformanceService, ApiPerformanceService>();
            // All mapping profiles live in this assembly
            builder.Services.AddAutoMapper(cfg => { }, typeof(CustomerProfile));

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ConcertsPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<MyBookingsPage>();
            builder.Services.AddTransient<BookingDetailsPage>();
            builder.Services.AddTransient<PerformancePage>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<PerformanceViewModel>();
            builder.Services.AddTransient<ConcertViewModel>();
            builder.Services.AddTransient<MyBookingsViewModel>();
            builder.Services.AddTransient<BookingDetailsViewModel>();
            return builder.Build();
        }
    }
}
