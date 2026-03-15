namespace OrbitToDo;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirm = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirm))
        {
            await DisplayAlert("⚠️ Hold On", "Please fill in all fields.", "OK");
            return;
        }

        if (!email.Contains("@"))
        {
            await DisplayAlert("⚠️ Invalid Email", "Please enter a valid email address.", "OK");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("⚠️ Mismatch", "Passwords do not match.", "OK");
            return;
        }

        if (AppSession.RegisteredUsers.Exists(u => u.email == email))
        {
            await DisplayAlert("⚠️ Already Registered", "This email is already in use.", "OK");
            return;
        }

        var newUser = new UserClass
        {
            user_id = AppSession.GetNextUserId(),
            username = username,
            email = email,
            password = password
        };

        AppSession.RegisteredUsers.Add(newUser);

        await DisplayAlert("🚀 Account Created!", "You can now sign in.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
