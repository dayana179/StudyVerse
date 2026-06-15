using System.Net.Http.Json;
using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Services
{
    public class ForumService
    {
        private readonly HttpClient _httpClient;

        public ForumService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl + "/")
            };
        }

        public async Task<List<ForumPostDto>> GetPostsAsync()
        {
            try
            {
                var posts = await _httpClient.GetFromJsonAsync<List<ForumPostDto>>("api/forum/posts");

                if (posts != null)
                {
                    AddFullAttachmentUrls(posts);
                }

                return posts ?? new List<ForumPostDto>();
            }
            catch
            {
                return new List<ForumPostDto>();
            }
        }

        public async Task<ForumPostDto?> GetPostDetailsAsync(int id)
        {
            try
            {
                var post = await _httpClient.GetFromJsonAsync<ForumPostDto>($"api/forum/posts/{id}");

                if (post != null)
                {
                    AddFullAttachmentUrls(new List<ForumPostDto> { post });
                }

                return post;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreatePostAsync(string title, string content, string category, List<FileResult> attachments)
        {
            try
            {
                using var form = new MultipartFormDataContent();

                form.Add(new StringContent(MobileUserSession.UserId), "UserId");
                form.Add(new StringContent(title), "Title");
                form.Add(new StringContent(content), "Content");
                form.Add(new StringContent(category), "Category");

                foreach (var file in attachments)
                {
                    var stream = await file.OpenReadAsync();
                    var fileContent = new StreamContent(stream);

                    string contentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType;

                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    form.Add(fileContent, "Attachments", file.FileName);
                }

                var response = await _httpClient.PostAsync("api/forum/posts", form);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EditPostAsync(int postId, string title, string content, string category, List<FileResult> attachments)
        {
            try
            {
                using var form = new MultipartFormDataContent();

                form.Add(new StringContent(MobileUserSession.UserId), "UserId");
                form.Add(new StringContent(title), "Title");
                form.Add(new StringContent(content), "Content");
                form.Add(new StringContent(category), "Category");

                foreach (var file in attachments)
                {
                    var stream = await file.OpenReadAsync();
                    var fileContent = new StreamContent(stream);

                    string contentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType;

                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    form.Add(fileContent, "Attachments", file.FileName);
                }

                var response = await _httpClient.PutAsync($"api/forum/posts/{postId}", form);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/forum/posts/{postId}?userId={Uri.EscapeDataString(userId)}"
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddReplyAsync(int postId, string content)
        {
            try
            {
                var request = new
                {
                    UserId = MobileUserSession.UserId,
                    Content = content
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"api/forum/posts/{postId}/replies",
                    request
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteReplyAsync(int replyId)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/forum/replies/{replyId}?userId={Uri.EscapeDataString(userId)}"
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private void AddFullAttachmentUrls(List<ForumPostDto> posts)
        {
            foreach (var post in posts)
            {
                foreach (var attachment in post.Attachments)
                {
                    if (!string.IsNullOrWhiteSpace(attachment.FilePath))
                    {
                        attachment.FullFileUrl = ApiSettings.GetFullUrl(attachment.FilePath);
                    }
                }
            }
        }
    }
}