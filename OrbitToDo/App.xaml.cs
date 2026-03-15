namespace OrbitToDo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new SignInPage())
        {
            BarBackgroundColor = Color.FromArgb("#060610"),
            BarTextColor = Color.FromArgb("#8892B0")
        };
    }
}
