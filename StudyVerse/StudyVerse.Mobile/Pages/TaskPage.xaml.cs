using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class TaskPage : ContentPage
{
    private readonly TaskService _taskService = new TaskService();

    private List<TaskItemDto> _tasks = new();

    public TaskPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!MobileUserSession.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        await LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        _tasks = await _taskService.GetTasksAsync();
        TaskCollection.ItemsSource = _tasks;
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadTasksAsync();
    }

    private async void CreateTaskClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateTaskPage());
    }

    private async void EditTaskClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int id)
        {
            return;
        }

        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return;
        }

        await Navigation.PushAsync(new EditTaskPage(task));
    }

    private async void DeleteTaskClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int id)
        {
            return;
        }

        bool confirm = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Delete", "Cancel");

        if (!confirm)
        {
            return;
        }

        bool success = await _taskService.DeleteTaskAsync(id);

        if (!success)
        {
            await DisplayAlert("Error", "Task could not be deleted.", "OK");
            return;
        }

        await LoadTasksAsync();
    }
}