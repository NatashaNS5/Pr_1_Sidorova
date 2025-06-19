using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Desktop.model;

namespace Desktop.model
{
    public class TodoApiClient : TodoHttpClient
    {
        public async Task<TodoModel[]> GetTodosAsync()
        {
            var client = GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Value);

            var response = await client.GetAsync("api/todos");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Ошибка при получении задач: {response.StatusCode} - {error}");
            }

            var todos = await response.Content.ReadFromJsonAsync<Response<TodoModel[]>>();
            return todos.Data;
        }

        public async Task<TodoModel> CreateTodoAsync(string title, string description, string category, DateTime date, bool isCompleted)
        {
            var client = GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Value);

            long timestamp = new DateTimeOffset(date).ToUnixTimeMilliseconds();

            var todoData = new
            {
                title = title,
                description = description,
                category = category,
                date = timestamp,
                isCompleted = isCompleted
            };

            var response = await client.PostAsJsonAsync("api/todos", todoData);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Ошибка при создании задачи: {response.StatusCode} - {error}");
            }

            var createdTodo = await response.Content.ReadFromJsonAsync<Response<TodoModel>>();
            return createdTodo.Data;
        }

        public async Task DeleteTodoAsync(string id)
        {
            var client = GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Value);

            var response = await client.DeleteAsync($"api/todos/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Ошибка при удалении задачи: {response.StatusCode} - {error}");
            }
        }
    }
}