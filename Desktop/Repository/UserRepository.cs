using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Todo.Entities;
using Desktop.model;
using System.IO;

namespace Desktop.Repository
{
    public class UserRepository
    {
        private static UserModel _currentUser;
        private readonly HttpClient _httpClient;

        public static UserModel CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public UserRepository()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://45.144.64.179/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Diagnostics.Debug.WriteLine($"TokenStorage.Value: {TokenStorage.Value}");
            if (!string.IsNullOrEmpty(TokenStorage.Value))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Value);
            }
        }

        public async Task<UserModel> GetUserInfo()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/user");
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"GetUserInfo response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var userResponse = System.Text.Json.JsonSerializer.Deserialize<Response<UserModel>>(responseContent, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true 
                    });
                    if (userResponse?.Data != null)
                    {
                        _currentUser = userResponse.Data;
                        return _currentUser;
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetUserInfo failed: {responseContent}");
                return _currentUser ?? new UserModel { Name = "Username" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUserInfo exception: {ex.Message}");
                return _currentUser ?? new UserModel { Name = "Username" };
            }
        }

        public async Task<(bool success, string errorMessage)> UploadUserPhoto(string filePath)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "uploadedFile", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync("api/user/photo", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"UploadUserPhoto response: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                await GetUserInfo();
                return (true, string.Empty);
            }
            return (false, responseContent);
        }

        public async Task<byte[]?> GetUserPhoto(string fileId)
        {
            var response = await _httpClient.GetAsync($"api/user/photo/{fileId}");
            System.Diagnostics.Debug.WriteLine($"GetUserPhoto response status: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }
    }
}