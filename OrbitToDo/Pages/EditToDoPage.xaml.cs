namespace OrbitToDo;

public partial class EditToDoPage : ContentPage
{
    private ToDoClass _todoItem;

    public EditToDoPage(ToDoClass item)
    {
        InitializeComponent();
        _todoItem = item;
        TitleEntry.Text   = item.item_name;
        DetailsEditor.Text = item.item_description;
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        string title   = TitleEntry.Text?.Trim();
        string details = DetailsEditor.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("⚠️ Required", "Title cannot be empty.", "OK");
            return;
        }

        SetLoading(true);

        // PUT /editItem_action.php
        var result = await ApiService.UpdateItemAsync(_todoItem.item_id, title, details);

        SetLoading(false);

        if (result.Success)
        {
            await DisplayAlert("✅ Updated", "Mission updated successfully.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", result.Message, "OK");
        }
    }

    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        SetLoading(true);

        // PUT /statusItem_action.php  { status: "inactive", item_id: ... }
        var result = await ApiService.ChangeStatusAsync(_todoItem.item_id, "inactive");

        SetLoading(false);

        if (result.Success)
        {
            await DisplayAlert("✨ Complete!", $"\"{_todoItem.item_name}\" marked as complete.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", result.Message, "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("🗑️ Delete", $"Delete \"{_todoItem.item_name}\"?", "Yes", "No");
        if (!confirm) return;

        SetLoading(true);

        // DELETE /deleteItem_action.php?item_id=...
        var result = await ApiService.DeleteItemAsync(_todoItem.item_id);

        SetLoading(false);

        if (result.Success)
            await Navigation.PopAsync();
        else
            await DisplayAlert("Error", result.Message, "OK");
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning  = isLoading;
        LoadingIndicator.IsVisible  = isLoading;
        UpdateButton.IsEnabled      = !isLoading;
        CompleteButton.IsEnabled    = !isLoading;
        DeleteButton.IsEnabled      = !isLoading;
    }
}
