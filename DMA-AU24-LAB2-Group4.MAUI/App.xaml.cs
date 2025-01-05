namespace DMA_AU24_LAB2_Group4.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Kontrollera om användaren är inloggad
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {
                MainPage = new AppShell();
                Shell.Current.GoToAsync("//ConcertsPage");
            }
            else
            {
                MainPage = new AppShell();
            }
        }
    }
}
