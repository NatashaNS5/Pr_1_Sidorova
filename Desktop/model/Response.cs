using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.model
{
    public class Response<T>
    {
        public T Data { get; set; }

        public Response() { }

        public Response(T data)
        {
            Data = data;
        }
    }
}