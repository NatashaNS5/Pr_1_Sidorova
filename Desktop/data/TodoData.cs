using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.data
{
    public class TodoData
    {
        public string id { get; set; }
        public string category { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public bool isCompleted { get; set; }
        public CoordinateData coordinate { get; set; }

    }
    public class CoordinateData
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
