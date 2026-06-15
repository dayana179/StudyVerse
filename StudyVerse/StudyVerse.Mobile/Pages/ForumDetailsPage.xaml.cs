using StudyVerse.Mobile.Services;
using StudyVerse.Mobile.Models;
using StudyVerse.Mobile.Helpers;

namespace StudyVerse.Mobile.Pages
{
    public partial class ForumDetailsPage : ContentPage
    {
        private readonly ForumService _forumService = new ForumService();
        private readonly int _postId;

        public ForumDetailsPage(int postId)
        {
            InitializeComponent();
            _postId = postId;
        }

        private async void OnAttachmentTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string fileUrl && !string.IsNullOrWhiteSpace(fileUrl))
            {
                await Launcher.OpenAsync(fileUrl);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var post = await _forumService.GetPostDetailsAsync(_postId);

            if (post == null)
            {
                await DisplayAlert("Error", "Discussion could not be loaded.", "OK");
                await Navigation.PopAsync();
                return;
            }



            TitleLabel.Text = post.Title;
            CategoryLabel.Text = string.IsNullOrWhiteSpace(post.Category) ? "General" : post.Category;
            ContentLabel.Text = post.Content;

            string baseUrl = "https://localhost:44329";
            if (post.Attachments != null)
            {
                foreach (var attachment in post.Attachments)
                {
                    if (!string.IsNullOrWhiteSpace(attachment.FilePath))
                    {
                        attachment.FullFileUrl = ApiSettings.GetFullUrl(attachment.FilePath);
                    }
                }
            }

            AttachmentCollection.ItemsSource = post.Attachments;
            AttachmentCollection.ItemsSource = post.Attachments;
            ReplyCollection.ItemsSource = post.Replies;
        }
    }
}