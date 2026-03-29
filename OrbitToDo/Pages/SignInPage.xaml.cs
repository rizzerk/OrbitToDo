namespace OrbitToDo;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email    = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        // Basic local validation first
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

        // Show loading
        SetLoading(true);

        // Call API: GET /signin_action.php?email=...&password=...
        var result = await ApiService.SignInAsync(email, password);

        SetLoading(false);

        if (result.Success)
        {
            // Store user in session
            AppSession.CurrentUser = result.Data;

            // Navigate to main tab page
            Application.Current.MainPage = new NavigationPage(new MainTabPage())
            {
                BarBackgroundColor = Color.FromArgb("#060610"),
                BarTextColor       = Color.FromArgb("#8892B0")
            };
        }
        else
        {
            await DisplayAlert("🚫 Sign In Failed", result.Message, "OK");
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning  = isLoading;
        LoadingIndicator.IsVisible  = isLoading;
        SignInButton.IsEnabled      = !isLoading;
    }
}
