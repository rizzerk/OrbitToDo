using System.Collections.ObjectModel;

namespace OrbitToDo;

public partial class CompletedPage : ContentPage
{
    private ObservableCollection<ToDoClass> _completedTodos = new();

    public CompletedPage()
    {
        InitializeComponent();
        CompletedListView.ItemsSource = _completedTodos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshListAsync();
    }

    public async Task RefreshListAsync()
    {
        // GET /getItems_action.php?status=inactive&user_id=...
        var result = await ApiService.GetItemsAsync("inactive", AppSession.CurrentUser.id);

        _completedTodos.Clear();
        if (result.Success && result.Data != null)
        {
            foreach (var item in result.Data)
                _completedTodos.Add(item);
        }
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is ToDoClass selectedItem)
        {
            CompletedListView.SelectedItem = null;
            await Navigation.PushAsync(new EditCompletedPage(selectedItem));
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
}
