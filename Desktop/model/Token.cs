using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.model
{
    public class Token
    {
        public string AccessToken { get; private set; }

        public Token(string access_token)
        {
            AccessToken = access_token;
        }
    }
}
