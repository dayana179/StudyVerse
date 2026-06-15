using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class TaskPage : ContentPage
{
    private readonly TaskService _taskService = new TaskService();

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
        var tasks = await _taskService.GetTasksAsync();
        TaskCollection.ItemsSource = tasks;
    }

    private async void AddTaskClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Missing title", "Please enter a task title.", "OK");
            return;
        }

        var task = new TaskItemDto
        {
            Title = TitleEntry.Text.Trim(),
            Description = DescriptionEditor.Text?.Trim(),
            DueDate = DueDatePicker.Date,
            IsCompleted = false
        };

        bool success = await _taskService.CreateTaskAsync(task);

        if (!success)
        {
            await DisplayAlert("Error", "Task could not be added. Make sure the MVC project is running.", "OK");
            return;
        }

        TitleEntry.Text = string.Empty;
        DescriptionEditor.Text = string.Empty;

        await LoadTasksAsync();
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadTasksAsync();
    }

    private async void ToggleTaskClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int id)
        {
            await _taskService.ToggleTaskAsync(id);
            await LoadTasksAsync();
        }
    }

    private async void DeleteTaskClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int id)
        {
            bool confirm = await DisplayAlert("Delete Task", "Are you sure you want to delete this task?", "Delete", "Cancel");

            if (!confirm)
            {
                return;
            }

            await _taskService.DeleteTaskAsync(id);
            await LoadTasksAsync();
        }
    }
}