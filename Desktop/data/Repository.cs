using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            private readonly string _connectionString = "Server=DESKTOP-KEHORP4;Database=TodoAppDB;Trusted_Connection=True;";

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

                    var response = await _httpClient.PostAsJsonAsync("api/auth/login", user);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<Response<Token>>(responseContent);
                        if (tokenResponse?.Data != null)
                        {
                            TokenStorage.Value = tokenResponse.Data.AccessToken;
                            TokenStorage.Username = email;

                            await SaveTokenToDatabase(email, TokenStorage.Value);
                            return (true, null);
                        }
                        else
                        {
                            return (false, "Не удалось десериализовать токен из ответа.");
                        }
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return (false, errorMessage);
                    }
                }
                catch (HttpRequestException)
                {
                    return (false, "Ошибка сети или сервера.");
                }
                catch (JsonException)
                {
                    return (false, "Неверный формат ответа от сервера.");
                }
                catch (Exception)
                {
                    return (false, "Произошла ошибка при попытке входа.");
                }
            }

            private async Task SaveTokenToDatabase(string email, string token)
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        var command = new SqlCommand(
                            @"MERGE UserTokens AS target
                              USING (SELECT @Email AS Email) AS source
                              ON (target.Email = source.Email)
                              WHEN MATCHED THEN
                                  UPDATE SET AccessToken = @AccessToken, LastUsed = @LastUsed
                              WHEN NOT MATCHED THEN
                                  INSERT (Email, AccessToken, LastUsed)
                                  VALUES (@Email, @AccessToken, @LastUsed);", connection);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@AccessToken", token);
                        command.Parameters.AddWithValue("@LastUsed", DateTime.Now);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<string> LoadTokenFromDatabase(string email)
            {
                try
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        var command = new SqlCommand("SELECT AccessToken FROM UserTokens WHERE Email = @Email", connection);
                        command.Parameters.AddWithValue("@Email", email);
                        var result = await command.ExecuteScalarAsync();
                        return result as string;
                    }
                }
                catch (SqlException)
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}