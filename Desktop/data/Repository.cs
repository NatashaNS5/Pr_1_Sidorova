using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

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
            private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

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
                            await SaveTokenToDatabase(email, TokenStorage.Value, false);
                            return (true, null);
                        }
                        return (false, "Не удалось десериализовать токен из ответа.");
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

            public async Task SetLoggedOutAsync(string email)
            {
                using (var connection = await _dbConnection.GetConnectionAsync())
                {
                    try
                    {
                        var command = new SqlCommand(
                            "UPDATE UserTokens SET IsLoggedOut = @IsLoggedOut WHERE Email = @Email",
                            connection);
                        command.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsLoggedOut", true);
                        await command.ExecuteNonQueryAsync();
                    }
                    finally
                    {
                        _dbConnection.CloseConnection(connection);
                    }
                }
            }

            private async Task SaveTokenToDatabase(string email, string token, bool isLoggedOut)
            {
                using (var connection = await _dbConnection.GetConnectionAsync())
                {
                    try
                    {
                        var command = new SqlCommand(
                            @"MERGE UserTokens AS target
                              USING (SELECT @Email AS Email) AS source
                              ON (target.Email = source.Email)
                              WHEN MATCHED THEN
                                  UPDATE SET AccessToken = @AccessToken, LastUsed = @LastUsed, IsLoggedOut = @IsLoggedOut
                              WHEN NOT MATCHED THEN
                                  INSERT (Email, AccessToken, LastUsed, IsLoggedOut)
                                  VALUES (@Email, @AccessToken, @LastUsed, @IsLoggedOut);", connection);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@AccessToken", token);
                        command.Parameters.AddWithValue("@LastUsed", DateTime.Now);
                        command.Parameters.AddWithValue("@IsLoggedOut", isLoggedOut);
                        await command.ExecuteNonQueryAsync();
                    }
                    finally
                    {
                        _dbConnection.CloseConnection(connection);
                    }
                }
            }

            public async Task<string> LoadTokenFromDatabase(string email)
            {
                using (var connection = await _dbConnection.GetConnectionAsync())
                {
                    try
                    {
                        var command = new SqlCommand("SELECT AccessToken FROM UserTokens WHERE Email = @Email", connection);
                        command.Parameters.AddWithValue("@Email", email);
                        var result = await command.ExecuteScalarAsync();
                        return result as string;
                    }
                    finally
                    {
                        _dbConnection.CloseConnection(connection);
                    }
                }
            }
        }
    }
}