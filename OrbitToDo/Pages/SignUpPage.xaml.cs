namespace OrbitToDo;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string firstName = FirstNameEntry.Text?.Trim();
        string lastName  = LastNameEntry.Text?.Trim();
        string email     = EmailEntry.Text?.Trim();
        string password  = PasswordEntry.Text;
        string confirm   = ConfirmPasswordEntry.Text;

        // Local validation
        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName)  ||
            string.IsNullOrWhiteSpace(email)     ||
            string.IsNullOrWhiteSpace(password)  ||
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

        // Show loading
        SetLoading(true);

        // Call API: POST /signup_action.php
        var result = await ApiService.SignUpAsync(firstName, lastName, email, password, confirm);

        SetLoading(false);

        if (result.Success)
        {
            await DisplayAlert("🚀 Account Created!", result.Message + "\nYou can now sign in.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("🚫 Sign Up Failed", result.Message, "OK");
        }
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        SignUpButton.IsEnabled     = !isLoading;
    }
}
