using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Desktop.model;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Desktop.data
{
    public class Repository : TodoHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string TodosUrl = "api/todos";

        public Repository()
        {
            _httpClient = GetHttpClient();
        }

        public async Task<Response<List<TodoData>>?> GetTodosAsync()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Value);
            return await _httpClient.GetFromJsonAsync<Response<List<TodoData>>>(TodosUrl);
        }
        public class AuthRepository : TodoHttpClient
        {
            private readonly HttpClient _httpClient;

            public AuthRepository()
            {
                _httpClient = GetHttpClient();
            }

            public async Task<(bool success, string errorMessage)> RegisterUserAsync(string username, string email, string password)
            {
                var user = new { Name = username, email, password };
                var response = await _httpClient.PostAsJsonAsync("api/auth/registration", user);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return (false, errorMessage);
                }
            }

            public async Task<(bool success, string errorMessage)> AuthenticateUserAsync(string email, string password)
            {
                try
                {
                    var user = new { Email = email, Password = password };
                    Console.WriteLine($"Отправка запроса с данными: {JsonConvert.SerializeObject(user)}");

                    var response = await _httpClient.PostAsJsonAsync("api/auth/login", user);

                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsAsync<Response<Token>>();
                        TokenStorage.Value = responseContent.Data.AccessToken;
                        TokenStorage.Username = email; // Сохраняем email как имя пользователя
                        Console.WriteLine($"Токен сохранен: {TokenStorage.Value}");

                        return (true, null);
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ошибка в ответе: {errorMessage}");
                        return (false, errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                    return (false, "Произошла ошибка при попытке входа.");
                }
            }
        }
    }
}