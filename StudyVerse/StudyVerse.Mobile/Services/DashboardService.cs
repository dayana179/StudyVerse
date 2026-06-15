using System.Net.Http.Json;
using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Services
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl + "/")
            };
        }

        public async Task<DashboardDto?> GetDashboardAsync()
        {
            try
            {
                string userId = MobileUserSession.UserId;

                return await _httpClient.GetFromJsonAsync<DashboardDto>(
                    $"api/dashboard?userId={Uri.EscapeDataString(userId)}"
                );
            }
            catch
            {
                return null;
            }
        }
    }
}