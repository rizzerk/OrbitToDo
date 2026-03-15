using System.Collections.ObjectModel;

namespace OrbitToDo;

public partial class CompletedPage : ContentPage
{
    private ObservableCollection<ToDoClass> _completedTodos;

    public CompletedPage()
    {
        InitializeComponent();
        _completedTodos = new ObservableCollection<ToDoClass>();
        CompletedListView.ItemsSource = _completedTodos;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshList();
    }

    private void RefreshList()
    {
        _completedTodos.Clear();
        foreach (var item in AppSession.TodoList)
        {
            if (item.status == "completed")
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
            if (confirm)
            {
                AppSession.TodoList.Remove(item);
                RefreshList();
            }
        }
    }
}
