using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class EditTaskPage : ContentPage
{
    private readonly TaskService _taskService = new TaskService();
    private readonly TaskItemDto _task;

    public EditTaskPage(TaskItemDto task)
    {
        InitializeComponent();

        _task = task;

        TitleEntry.Text = task.Title;
        DueDatePicker.Date = task.DueDate ?? DateTime.Today;

        PriorityPicker.SelectedItem = string.IsNullOrWhiteSpace(task.Priority)
            ? "Medium"
            : task.Priority;

        StatusPicker.SelectedItem = task.IsCompleted ? "Completed" : "Pending";
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Missing Title", "Please enter a task title.", "OK");
            return;
        }

        _task.Title = TitleEntry.Text.Trim();
        _task.DueDate = DueDatePicker.Date;
        _task.Priority = PriorityPicker.SelectedItem?.ToString() ?? "Medium";
        _task.IsCompleted = StatusPicker.SelectedItem?.ToString() == "Completed";

        bool success = await _taskService.EditTaskAsync(_task);

        if (!success)
        {
            await DisplayAlert("Error", "Task could not be updated.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}