using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Desktop.Utiles
{
    static class InputValidator
    {
        private static readonly string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public static bool ValidateEmail(this string email)
        {
            return Regex.IsMatch(email, emailPattern);
        }

        public static bool ValidatePassword(this string password)
        {
            return password.Length >= 6;
        }
        public static bool ValidateUsername(this string username)
        {
            return username.Length >= 3;
        }
    }
}
