using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Services;

namespace StudyVerse.Mobile.Pages;

public partial class EditForumPostPage : ContentPage
{
    private readonly ForumService _forumService = new ForumService();
    private readonly ForumPostDto _post;
    private readonly List<FileResult> _selectedFiles = new();

    public EditForumPostPage(ForumPostDto post)
    {
        InitializeComponent();

        _post = post;

        TitleEntry.Text = post.Title;
        ContentEditor.Text = post.Content;

        var categories = new List<string>
        {
            "General",
            "Question",
            "Notes",
            "Announcement"
        };

        int index = categories.IndexOf(post.Category ?? "General");
        CategoryPicker.SelectedIndex = index >= 0 ? index : 0;

        SelectedFilesCollection.ItemsSource = _selectedFiles;
    }

    private async void ChooseAttachmentClicked(object sender, EventArgs e)
    {
        try
        {
            var files = await FilePicker.Default.PickMultipleAsync();

            if (files == null)
            {
                return;
            }

            foreach (var file in files)
            {
                _selectedFiles.Add(file);
            }

            SelectedFilesCollection.ItemsSource = null;
            SelectedFilesCollection.ItemsSource = _selectedFiles;
        }
        catch
        {
            await DisplayAlert("Error", "Could not choose attachment.", "OK");
        }
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Missing Title", "Please enter a post title.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(ContentEditor.Text))
        {
            await DisplayAlert("Missing Content", "Please enter post content.", "OK");
            return;
        }

        string category = CategoryPicker.SelectedItem?.ToString() ?? "General";

        bool success = await _forumService.EditPostAsync(
            _post.Id,
            TitleEntry.Text.Trim(),
            ContentEditor.Text.Trim(),
            category,
            _selectedFiles
        );

        if (!success)
        {
            await DisplayAlert("Error", "Post could not be updated.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}