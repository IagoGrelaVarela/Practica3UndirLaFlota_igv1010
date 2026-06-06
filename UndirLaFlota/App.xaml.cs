namespace UndirLaFlota
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Shell.Current.GoToAsync("//LoginPage"); // Iniciamos en la página de Login
        }
    }
}
