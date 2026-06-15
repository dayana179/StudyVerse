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
                string userId = MobileUserSession.UserId;

                var tasks = await _httpClient.GetFromJsonAsync<List<TaskItemDto>>(
                    $"api/tasks?userId={Uri.EscapeDataString(userId)}"
                );

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
                string userId = MobileUserSession.UserId;

                var request = new
                {
                    UserId = userId,
                    task.Title,
                    task.DueDate,
                    task.Priority,
                    task.IsCompleted
                };

                var response = await _httpClient.PostAsJsonAsync("api/tasks", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EditTaskAsync(TaskItemDto task)
        {
            try
            {
                string userId = MobileUserSession.UserId;

                var request = new
                {
                    UserId = userId,
                    task.Title,
                    task.DueDate,
                    task.Priority,
                    task.IsCompleted
                };

                var response = await _httpClient.PutAsJsonAsync($"api/tasks/{task.Id}", request);
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
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.PutAsync(
                    $"api/tasks/{id}/toggle?userId={Uri.EscapeDataString(userId)}",
                    null
                );

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
                string userId = MobileUserSession.UserId;

                var response = await _httpClient.DeleteAsync(
                    $"api/tasks/{id}?userId={Uri.EscapeDataString(userId)}"
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