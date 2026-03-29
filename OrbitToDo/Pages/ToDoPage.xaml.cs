using System.Collections.ObjectModel;

namespace OrbitToDo;

public partial class ToDoPage : ContentPage
{
    private ObservableCollection<ToDoClass> _activeTodos = new();

    public ToDoPage()
    {
        InitializeComponent();
        TodoListView.ItemsSource = _activeTodos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshListAsync();
    }

    public async Task RefreshListAsync()
    {
        // GET /getItems_action.php?status=active&user_id=...
        var result = await ApiService.GetItemsAsync("active", AppSession.CurrentUser.id);

        _activeTodos.Clear();
        if (result.Success && result.Data != null)
        {
            foreach (var item in result.Data)
                _activeTodos.Add(item);
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddToDoPage());
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is ToDoClass selectedItem)
        {
            TodoListView.SelectedItem = null;
            await Navigation.PushAsync(new EditToDoPage(selectedItem));
        }
    }

    private async void OnDeleteTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is ToDoClass item)
        {
            bool confirm = await DisplayAlert("🗑️ Delete", $"Delete \"{item.item_name}\"?", "Yes", "No");
            if (!confirm) return;

            // DELETE /deleteItem_action.php?item_id=...
            var result = await ApiService.DeleteItemAsync(item.item_id);
            if (result.Success)
                await RefreshListAsync();
            else
                await DisplayAlert("Error", result.Message, "OK");
        }
    }

    private async void OnCompleteTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is ToDoClass item)
        {
            // PUT /statusItem_action.php  { status: "inactive", item_id: ... }
            var result = await ApiService.ChangeStatusAsync(item.item_id, "inactive");
            if (result.Success)
            {
                await RefreshListAsync();
                await DisplayAlert("✨ Done!", $"\"{item.item_name}\" marked as complete.", "OK");
            }
            else
            {
                await DisplayAlert("Error", result.Message, "OK");
            }
        }
    }
}
