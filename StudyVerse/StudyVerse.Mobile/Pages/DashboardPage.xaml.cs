namespace StudyVerse.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    private async void OpenForumClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ForumPage");
    }

    private async void OpenTasksClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TaskPage");
    }

    private async void OpenFlashcardsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//FlashcardsPage");
    }

    private async void OpenPomodoroClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PomodoroPage");
    }
}