using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Entities;

namespace Desktop.Repository
{
    public class UserRepository
    {
        private static List<UserModel> _users = new List<UserModel>();

        // Метод для регистрации пользователя
        public static bool RegisterUser(string username, string email, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Проверка уникальности имени пользователя
            if (_users.Any(user => user.Username == username))
            {
                errorMessage = "Имя пользователя уже занято.";
                return false;
            }

            // Добавление пользователя в список
            _users.Add(new UserModel
            {
                Username = username,
                Email = email,
                Password = password
            });

            return true;
        }

        // Метод для авторизации пользователя
        public static bool AuthenticateUser(string email, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            var user = _users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                errorMessage = "Неверное имя пользователя или пароль.";
                return false;
            }

            return true;
        }
    }
}