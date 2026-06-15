using System.Net.Http.Json;
using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl + "/")
            };
        }

        public async Task<LoginResponseDto?> LoginAsync(string email, string password)
        {
            try
            {
                var login = new LoginDto
                {
                    Email = email,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/login", login);

                if (!response.IsSuccessStatusCode)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Invalid login attempt."
                    };
                }

                return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            }
            catch
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Could not connect to StudyVerse server."
                };
            }
        }
    }
}