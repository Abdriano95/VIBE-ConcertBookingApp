using DMA_AU24_LAB2_Group4.MAUI.Views;
namespace DMA_AU24_LAB2_Group4.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(BookingListPage), typeof(BookingListPage));
            Routing.RegisterRoute(nameof(ConcertsPage), typeof(ConcertsPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(MyBookingsPage), typeof(MyBookingsPage));

        }
    }
}
