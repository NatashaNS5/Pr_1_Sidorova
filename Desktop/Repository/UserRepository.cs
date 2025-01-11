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
        public static UserModel CurrentUser { get; private set; } 

        public static bool RegisterUser(string username, string email, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_users.Any(user => user.Username == username))
            {
                errorMessage = "Имя пользователя уже занято.";
                return false;
            }

            var newUser = new UserModel
            {
                Username = username,
                Email = email,
                Password = password
            };
            _users.Add(newUser);

            CurrentUser = newUser;

            return true;
        }

        public static bool AuthenticateUser(string email, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            var user = _users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                errorMessage = "Неверное имя пользователя или пароль.";
                return false;
            }

            CurrentUser = user;

            return true;
        }
    }
}