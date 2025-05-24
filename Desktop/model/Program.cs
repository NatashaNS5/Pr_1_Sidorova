using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Desktop.model
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://45.144.64.179/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var repository = new Desktop.data.Repository();
            try
            {
                var user = new User(email: "string", password: "string");
                var response = await client.PostAsJsonAsync("api/auth/login", user);
                response.EnsureSuccessStatusCode();

                TokenStorage.Value = (await response.Content.ReadFromJsonAsync<Response<Token>>()).Data.AccessToken;

                var result = await repository.GetTodosAsync();
                if (result != null)
                {
                    var todos = result.Data;
                    var todoModel = todos.Select(todo => TodoModel.Map(todo)).ToList();

                    Console.WriteLine(result.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}