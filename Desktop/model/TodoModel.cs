using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desktop.data;

namespace Desktop.model
{
    public class TodoModel
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime date { get; set; }
        public bool IsCompleted { get; set; }
        public CoordinateData Coordinate { get; set; }

        public static TodoModel Map(TodoData model)
        {
            return new TodoModel
            {
                Id = model.id,
                Category = model.category,
                Title = model.title,
                Description = model.description,
                date = model.date,
                IsCompleted = model.isCompleted,
            };
        }
    }

    public class CoordinateData
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}