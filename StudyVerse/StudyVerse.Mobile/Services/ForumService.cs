using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;
using System.Net.Http.Json;

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
                return await _httpClient.GetFromJsonAsync<ForumPostDto>($"api/forum/posts/{id}");
            }
            catch
            {
                return null;
            }
        }
    }
}