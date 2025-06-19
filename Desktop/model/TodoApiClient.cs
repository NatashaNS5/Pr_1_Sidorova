using Desktop.model;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
                System.Diagnostics.Debug.WriteLine($"GetTodosAsync error: {response.StatusCode} - {error}");
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
                System.Diagnostics.Debug.WriteLine($"CreateTodoAsync error: {response.StatusCode} - {error}");
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
                System.Diagnostics.Debug.WriteLine($"DeleteTodoAsync error: {response.StatusCode} - {error}");
                throw new HttpRequestException($"Ошибка при удалении задачи: {response.StatusCode} - {error}");
            }
        }

        public async Task<TodoModel> UpdateTodoAsync(string id, bool isCompleted)
        {
            var client = GetHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Value);

            var todo = (await GetTodosAsync()).FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTodoAsync: Task with id {id} not found");
                throw new HttpRequestException("Задача не найдена");
            }

            long timestamp = new DateTimeOffset(DateTimeOffset.FromUnixTimeMilliseconds(todo.date).DateTime).ToUnixTimeMilliseconds();

            var todoData = new
            {
                id = id,
                title = todo.Title,
                description = todo.Description,
                category = todo.Category,
                date = timestamp,
                isCompleted = isCompleted
            };

            var response = await client.PutAsJsonAsync($"api/todos/mark/{id}", todoData); 
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"UpdateTodoAsync request: {JsonSerializer.Serialize(todoData)}");
                System.Diagnostics.Debug.WriteLine($"UpdateTodoAsync raw response: {error}");
                throw new HttpRequestException($"Ошибка при обновлении задачи: {response.StatusCode} - {error}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"UpdateTodoAsync JSON response: {jsonString}");

            if (string.IsNullOrEmpty(jsonString) || !jsonString.TrimStart().StartsWith("{") && !jsonString.TrimStart().StartsWith("["))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid JSON response: {jsonString}");
                if (response.IsSuccessStatusCode)
                {
                    return new TodoModel { Id = id, Title = todo.Title, Description = todo.Description, Category = todo.Category, date = timestamp, IsCompleted = isCompleted };
                }
                throw new JsonException("Сервер вернул некорректный JSON-ответ.");
            }

            var updatedTodo = await response.Content.ReadFromJsonAsync<Response<TodoModel>>();
            return updatedTodo.Data;
        }
    }
}