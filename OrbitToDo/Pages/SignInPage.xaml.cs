namespace OrbitToDo;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("⚠️ Hold On", "Please fill in all fields.", "OK");
            return;
        }

        if (!email.Contains("@"))
        {
            await DisplayAlert("⚠️ Invalid Email", "Please enter a valid email address.", "OK");
            return;
        }

        // Check registered users or allow any credentials (demo mode)
        var user = AppSession.RegisteredUsers.Find(u => u.email == email && u.password == password);

        if (user == null)
        {
            // Demo: create a session user if not registered
            user = new UserClass
            {
                user_id = AppSession.GetNextUserId(),
                username = email.Split('@')[0],
                email = email,
                password = password
            };
        }

        AppSession.CurrentUser = user;

        // Navigate to main tab page
        Application.Current.MainPage = new NavigationPage(new MainTabPage())
        {
            BarBackgroundColor = Color.FromArgb("#060610"),
            BarTextColor = Color.FromArgb("#8892B0")
        };
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}
