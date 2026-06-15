using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardService _dashboardService = new DashboardService();

    public DashboardPage()
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

        WelcomeLabel.Text = MobileUserSession.UserName;

        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();

        if (dashboard == null)
        {
            await DisplayAlert("Dashboard Error", "Could not load dashboard data. Make sure the MVC project is running.", "OK");
            return;
        }

        TotalTasksLabel.Text = dashboard.TotalTasks.ToString();
        CompletedTasksLabel.Text = dashboard.CompletedTasks.ToString();
        PendingTasksLabel.Text = dashboard.PendingTasks.ToString();
        ProgressLabel.Text = $"{dashboard.Progress}%";
        ProgressBarLabel.Text = $"{dashboard.Progress}%";

        double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        double availableWidth = screenWidth - 36;

        ProgressFill.WidthRequest = availableWidth * dashboard.Progress / 100.0;

        TodayTasksCollection.ItemsSource = dashboard.TodayTasks;
        UpcomingTasksCollection.ItemsSource = dashboard.UpcomingTasks;
        RecentDecksCollection.ItemsSource = dashboard.RecentDecks;
        RecentPostsCollection.ItemsSource = dashboard.RecentPosts;
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadDashboardAsync();
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

    private async void OpenChatClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ChatPage");
    }

    private async void LogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to log out?", "Logout", "Cancel");

        if (!confirm)
        {
            return;
        }

        MobileUserSession.Clear();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}