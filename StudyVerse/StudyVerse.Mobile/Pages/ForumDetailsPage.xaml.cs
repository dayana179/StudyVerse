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
            RepliesCollection.ItemsSource = post.Replies;
        }

        private async void AddReplyClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReplyEntry.Text))
            {
                return;
            }

            bool success = await _forumService.AddReplyAsync(_postId, ReplyEntry.Text.Trim());

            if (!success)
            {
                await DisplayAlert("Error", "Reply could not be added.", "OK");
                return;
            }

            ReplyEntry.Text = string.Empty;

            var post = await _forumService.GetPostDetailsAsync(_postId);

            if (post != null)
            {
                RepliesCollection.ItemsSource = post.Replies;
            }
        }

        private async void DeleteReplyTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter == null)
            {
                return;
            }

            int replyId = Convert.ToInt32(e.Parameter);

            bool confirm = await DisplayAlert(
                "Delete Reply",
                "Are you sure you want to delete this reply?",
                "Delete",
                "Cancel"
            );

            if (!confirm)
            {
                return;
            }

            bool success = await _forumService.DeleteReplyAsync(replyId);

            if (!success)
            {
                await DisplayAlert("Not allowed", "You can only delete your own replies.", "OK");
                return;
            }

            var post = await _forumService.GetPostDetailsAsync(_postId);

            if (post != null)
            {
                RepliesCollection.ItemsSource = post.Replies;
            }
        }
    }
}