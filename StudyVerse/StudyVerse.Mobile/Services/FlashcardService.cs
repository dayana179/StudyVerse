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

        public async Task<List<FlashcardDeckDto>> GetDecksAsync()
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var decks = await _httpClient.GetFromJsonAsync<List<FlashcardDeckDto>>(
                    $"api/flashcards/decks?userId={Uri.EscapeDataString(userId)}"
                );

                return decks ?? new List<FlashcardDeckDto>();
            }
            catch
            {
                return new List<FlashcardDeckDto>();
            }
        }

        public async Task<bool> CreateDeckAsync(string name, string? description)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var request = new
                {
                    UserId = userId,
                    Name = name,
                    Description = description
                };

                var response = await _httpClient.PostAsJsonAsync("api/flashcards/decks", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteDeckAsync(int deckId)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/flashcards/decks/{deckId}?userId={Uri.EscapeDataString(userId)}"
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<FlashcardDto>> GetCardsAsync(int deckId)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var cards = await _httpClient.GetFromJsonAsync<List<FlashcardDto>>(
                    $"api/flashcards/decks/{deckId}/cards?userId={Uri.EscapeDataString(userId)}"
                );

                return cards ?? new List<FlashcardDto>();
            }
            catch
            {
                return new List<FlashcardDto>();
            }
        }

        public async Task<bool> CreateCardAsync(int deckId, string question, string answer)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var request = new
                {
                    UserId = userId,
                    Question = question,
                    Answer = answer
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"api/flashcards/decks/{deckId}/cards",
                    request
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCardAsync(int cardId)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/flashcards/cards/{cardId}?userId={Uri.EscapeDataString(userId)}"
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