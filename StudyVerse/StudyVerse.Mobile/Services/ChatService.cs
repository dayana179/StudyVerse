using System.Net.Http.Json;
using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;

        public ChatService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl + "/")
            };
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync()
        {
            try
            {
                var messages = await _httpClient.GetFromJsonAsync<List<ChatMessageDto>>("api/chat/messages");
                return messages ?? new List<ChatMessageDto>();
            }
            catch
            {
                return new List<ChatMessageDto>();
            }
        }

        public async Task<bool> SendMessageAsync(string message)
        {
            try
            {
                var request = new
                {
                    UserId = MobileUserSession.UserId,
                    Message = message,
                    ForumPostId = (int?)null,
                    ForumPostTitle = (string?)null
                };

                var response = await _httpClient.PostAsJsonAsync("api/chat/send", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/chat/{id}?userId={Uri.EscapeDataString(userId)}"
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}