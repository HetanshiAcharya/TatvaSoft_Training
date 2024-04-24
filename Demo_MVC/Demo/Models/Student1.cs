using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Student1
    {

        public int Id { get; set; }


        public string Name { get; set; } = null!;


        public string Department { get; set; } = null!;

        public int? Semester { get; set; }

        public int? Age { get; set; }

        public int? Fees { get; set; }


       
        public IFormFile? PhotoId { get; set; }

    }
}