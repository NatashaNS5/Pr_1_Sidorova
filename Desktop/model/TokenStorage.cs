using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.model
{
    public static class TokenStorage
    {
        private const string TokenFilePath = "tokenstorage.txt";
        public static string Username { get; set; }
        public static string Value { get; set; }

        public static void Save()
        {
            File.WriteAllLines(TokenFilePath, new[] { Username ?? "", Value ?? "" });
        }

        public static void Load()
        {
            if (File.Exists(TokenFilePath))
            {
                var lines = File.ReadAllLines(TokenFilePath);
                if (lines.Length >= 2)
                {
                    Username = lines[0];
                    Value = lines[1];
                }
            }
        }
    }
}