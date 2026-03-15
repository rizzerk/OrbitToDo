namespace OrbitToDo;

public partial class EditCompletedPage : ContentPage
{
    private ToDoClass _todoItem;

    public EditCompletedPage(ToDoClass item)
    {
        InitializeComponent();
        _todoItem = item;
        TitleEntry.Text = item.item_name;
        DetailsEditor.Text = item.item_description;
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("⚠️ Required", "Title cannot be empty.", "OK");
            return;
        }

        _todoItem.item_name = title;
        _todoItem.item_description = DetailsEditor.Text?.Trim() ?? string.Empty;

        await DisplayAlert("✅ Updated", "Mission updated successfully.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnIncompleteClicked(object sender, EventArgs e)
    {
        _todoItem.status = "todo";
        await DisplayAlert("🔄 Moved", $"\"{_todoItem.item_name}\" moved back to active missions.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("🗑️ Delete", $"Delete \"{_todoItem.item_name}\"?", "Yes", "No");
        if (confirm)
        {
            AppSession.TodoList.Remove(_todoItem);
            await Navigation.PopAsync();
        }
    }
}
