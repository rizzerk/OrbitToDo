namespace OrbitToDo;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (AppSession.CurrentUser != null)
        {
            UsernameLabel.Text = AppSession.CurrentUser.username;
            EmailLabel.Text = AppSession.CurrentUser.email;
        }
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
        if (confirm)
        {
            AppSession.CurrentUser = null;
            AppSession.TodoList.Clear();

            Application.Current.MainPage = new NavigationPage(new SignInPage())
            {
                BarBackgroundColor = Color.FromArgb("#060610"),
                BarTextColor = Color.FromArgb("#8892B0")
            };
        }
    }
}
