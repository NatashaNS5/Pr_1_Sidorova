using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.model
{
    public abstract class TodoHttpClient
    {
        private readonly Uri _baseUri;
        protected TodoHttpClient()
        {
            _baseUri = new Uri("http://45.144.64.179/");
        }
        protected HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = _baseUri;
            return client;
        }
    }
}
