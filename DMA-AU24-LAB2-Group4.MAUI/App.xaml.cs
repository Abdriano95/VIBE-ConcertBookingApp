namespace DMA_AU24_LAB2_Group4.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // navigate based on login status
            MainPage = new AppShell();
            NavigateBasedOnLoginStatus();
        }

        private async void NavigateBasedOnLoginStatus()
        {
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
            if (isLoggedIn)
            {
                await Shell.Current.GoToAsync("//ConcertsPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
