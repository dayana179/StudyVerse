using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class ForumPage : ContentPage
{
    private readonly ForumService _forumService = new ForumService();

    private List<ForumPostDto> _posts = new();

    public ForumPage()
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

        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        _posts = await _forumService.GetPostsAsync();
        ForumCollection.ItemsSource = _posts;
    }

    private async void RefreshClicked(object sender, EventArgs e)
    {
        await LoadPostsAsync();
    }

    private async void CreatePostClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateForumPostPage());
    }

    private async void OpenPostClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int postId)
        {
            return;
        }

        await Navigation.PushAsync(new ForumDetailsPage(postId));
    }

    private async void EditPostClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int postId)
        {
            return;
        }

        var post = _posts.FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            return;
        }

        if (!post.IsOwnPost)
        {
            await DisplayAlert("Not allowed", "You can only edit your own post.", "OK");
            return;
        }

        await Navigation.PushAsync(new EditForumPostPage(post));
    }

    private async void DeletePostClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int postId)
        {
            return;
        }

        var post = _posts.FirstOrDefault(p => p.Id == postId);

        if (post == null)
        {
            return;
        }

        if (!post.IsOwnPost)
        {
            await DisplayAlert("Not allowed", "You can only delete your own post.", "OK");
            return;
        }

        bool confirm = await DisplayAlert(
            "Delete Post",
            "Are you sure you want to delete this forum post?",
            "Delete",
            "Cancel"
        );

        if (!confirm)
        {
            return;
        }

        bool success = await _forumService.DeletePostAsync(postId);

        if (!success)
        {
            await DisplayAlert("Error", "Post could not be deleted.", "OK");
            return;
        }

        await LoadPostsAsync();
    }
}