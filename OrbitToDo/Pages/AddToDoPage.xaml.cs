namespace OrbitToDo;

public partial class AddToDoPage : ContentPage
{
    public AddToDoPage()
    {
        InitializeComponent();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        string title   = TitleEntry.Text?.Trim();
        string details = DetailsEditor.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("⚠️ Required", "Please enter a title for your mission.", "OK");
            return;
        }

        SetLoading(true);

        // POST /addItem_action.php
        var result = await ApiService.AddItemAsync(title, details, AppSession.CurrentUser.id);

        SetLoading(false);

        if (result.Success)
        {
            await DisplayAlert("🚀 Added!", $"\"{title}\" added to your missions.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", result.Message, "OK");
        }
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        AddButton.IsEnabled        = !isLoading;
    }
}
