using System.Collections.ObjectModel;

namespace OrbitToDo;

public partial class ToDoPage : ContentPage
{
    private ObservableCollection<ToDoClass> _activeTodos;

    public ToDoPage()
    {
        InitializeComponent();
        _activeTodos = new ObservableCollection<ToDoClass>();
        TodoListView.ItemsSource = _activeTodos;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshList();
    }

    private void RefreshList()
    {
        _activeTodos.Clear();
        foreach (var item in AppSession.TodoList)
        {
            if (item.status == "todo")
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
            if (confirm)
            {
                AppSession.TodoList.Remove(item);
                RefreshList();
            }
        }
    }

    private async void OnCompleteTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is ToDoClass item)
        {
            item.status = "completed";
            RefreshList();
            await DisplayAlert("✨ Done!", $"\"{item.item_name}\" marked as complete.", "OK");
        }
    }
}
