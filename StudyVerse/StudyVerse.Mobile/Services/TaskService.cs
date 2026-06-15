using System.Net.Http.Json;
using StudyVerse.Mobile.Helpers;
using StudyVerse.Mobile.Models;

namespace StudyVerse.Mobile.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;

        public TaskService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiSettings.BaseUrl + "/")
            };
        }

        public async Task<List<TaskItemDto>> GetTasksAsync()
        {
            try
            {
                var tasks = await _httpClient.GetFromJsonAsync<List<TaskItemDto>>("api/tasks");
                return tasks ?? new List<TaskItemDto>();
            }
            catch
            {
                return new List<TaskItemDto>();
            }
        }

        public async Task<bool> CreateTaskAsync(TaskItemDto task)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/tasks", task);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleTaskAsync(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/tasks/{id}/toggle", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}