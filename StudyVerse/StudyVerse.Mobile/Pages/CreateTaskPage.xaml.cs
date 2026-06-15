using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class CreateTaskPage : ContentPage
{
    private readonly TaskService _taskService = new TaskService();

    public CreateTaskPage()
    {
        InitializeComponent();
        PriorityPicker.SelectedIndex = 1;
    }

    private async void CreateClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Missing Title", "Please enter a task title.", "OK");
            return;
        }

        var task = new TaskItemDto
        {
            Title = TitleEntry.Text.Trim(),
            DueDate = DueDatePicker.Date,
            Priority = PriorityPicker.SelectedItem?.ToString() ?? "Medium",
            IsCompleted = false
        };

        bool success = await _taskService.CreateTaskAsync(task);

        if (!success)
        {
            await DisplayAlert("Error", "Task could not be created.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}