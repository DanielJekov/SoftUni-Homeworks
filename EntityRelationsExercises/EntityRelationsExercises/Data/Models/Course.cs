using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("Courses")]
    public class Course
    {
        public Course()
        {
            this.StudentsEnrolled = new HashSet<StudentCourse>();
            this.Resources = new HashSet<Resource>();
            this.HomeworkSubmissions = new HashSet<Homework>();
        }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Price { get; set; }
        public ICollection<StudentCourse> StudentsEnrolled { get; set; }
        public ICollection<Resource> Resources { get; set; }
        public ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}


