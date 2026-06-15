using System.Net.Http.Json;
using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Services
{
    public class FlashcardService
    {
        private readonly HttpClient _httpClient;

        public FlashcardService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl + "/")
            };
        }

        public async Task<List<FlashcardDto>> GetFlashcardsAsync()
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var cards = await _httpClient.GetFromJsonAsync<List<FlashcardDto>>(
                    $"api/flashcards?userId={Uri.EscapeDataString(userId)}"
                );

                return cards ?? new List<FlashcardDto>();
            }
            catch
            {
                return new List<FlashcardDto>();
            }
        }

        public async Task<bool> CreateFlashcardAsync(FlashcardDto flashcard)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var request = new
                {
                    UserId = userId,
                    flashcard.Question,
                    flashcard.Answer
                };

                var response = await _httpClient.PostAsJsonAsync("api/flashcards", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFlashcardAsync(int id)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/flashcards/{id}?userId={Uri.EscapeDataString(userId)}"
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