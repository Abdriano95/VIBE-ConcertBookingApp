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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Services
            builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
            builder.Services.AddSingleton<IRestService, RestService>();
            builder.Services.AddSingleton<IBookingService, BookingService>();
            builder.Services.AddSingleton<ICustomerService, CustomerService>();
            builder.Services.AddSingleton<IConcertService, ConcertService>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddAutoMapper(typeof(CustomerProfile));

            // Pages
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<BookingListPage>();
            builder.Services.AddTransient<BookingItemPage>();
            builder.Services.AddSingleton<ConcertsPage>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<ProfilePage>();
            builder.Services.AddSingleton<MyBookingsPage>();

            // ViewModels
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<BookingListViewModel>();
            builder.Services.AddTransient<BookingItemViewModel>();
            builder.Services.AddSingleton<ConcertViewModel>();
            return builder.Build();
        }
    }
}
