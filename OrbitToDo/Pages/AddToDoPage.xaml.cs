namespace OrbitToDo;

public partial class AddToDoPage : ContentPage
{
    public AddToDoPage()
    {
        InitializeComponent();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim();
        string details = DetailsEditor.Text?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("⚠️ Required", "Please enter a title for your mission.", "OK");
            return;
        }

        var newTodo = new ToDoClass
        {
            item_id = AppSession.GetNextId(),
            item_name = title,
            item_description = details ?? string.Empty,
            status = "todo",
            user_id = AppSession.CurrentUser?.user_id ?? 1
        };

        AppSession.TodoList.Add(newTodo);

        await DisplayAlert("🚀 Added!", $"\"{title}\" added to your missions.", "OK");
        await Navigation.PopAsync();
    }
}
