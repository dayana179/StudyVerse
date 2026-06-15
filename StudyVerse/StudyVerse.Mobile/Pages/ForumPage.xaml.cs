using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class ForumPage : ContentPage
{
    private readonly ForumService _forumService = new ForumService();

    public ForumPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        var posts = await _forumService.GetPostsAsync();
        ForumCollection.ItemsSource = posts;
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadPostsAsync();
    }

    private async void OnOpenDiscussionClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int postId)
        {
            await Navigation.PushAsync(new ForumDetailsPage(postId));
        }
    }
}