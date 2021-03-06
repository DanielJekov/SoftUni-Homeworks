using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.; Database = StudentSystem; Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Modeling mapping table
            modelBuilder.Entity<StudentCourse>()
                .HasKey(x => new { x.CourseId, x.StudentId });

            // Modeling Studen Class
            modelBuilder.Entity<Student>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired();

            modelBuilder.Entity<Student>()
                .Property(x => x.PhoneNumber)
                .HasMaxLength(10)
                .IsRequired(false)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(x => x.RegisteredOn)
                .IsRequired();

            //Modeling Course Class
            modelBuilder.Entity<Course>()
                .Property(x => x.Name)
                .HasMaxLength(80)
                .IsUnicode()
                .IsRequired();

            modelBuilder.Entity<Course>()
                .Property(x => x.Description)
                .IsUnicode();

            //Modeling Resource Class
            modelBuilder.Entity<Resource>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsUnicode(true)
                .IsRequired();

            modelBuilder.Entity<Resource>()
                .Property(x => x.Url)
                .IsRequired();

            modelBuilder.Entity<Resource>()
                .Property(x => x.ResourceType)
                .IsRequired();

            //Modeling Homework Class
            modelBuilder.Entity<Homework>()
                .Property(x => x.Content)
                .IsRequired();

            modelBuilder.Entity<Homework>()
                .Property(x => x.ContentType)
                .IsRequired();




            base.OnModelCreating(modelBuilder);
        }
    }
}