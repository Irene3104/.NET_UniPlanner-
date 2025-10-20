using Microsoft.EntityFrameworkCore;
using UniPlanner.Models;
using System.IO;

namespace UniPlanner.Data
{
    /// <summary>
    /// Entity Framework DbContext for UniPlanner
    /// Provides ORM capabilities for database operations
    /// </summary>
    public class UniPlannerContext : DbContext
    {
        public UniPlannerContext()
        {
        }

        public UniPlannerContext(DbContextOptions<UniPlannerContext> options) : base(options)
        {
        }

        // DbSets for each entity
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<ScheduleItem> Schedules { get; set; } = null!;
        public DbSet<TodoItem> Todos { get; set; } = null!;
        public DbSet<SubjectItem> Subjects { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Reference Data folder in project root
                var projectRoot = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                var dbPath = Path.Combine(projectRoot, "Data", "uni.db");
                
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                optionsBuilder.UseSqlite($"Data Source={dbPath};Foreign Keys=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure TaskItem
            modelBuilder.Entity<TaskItem>().ToTable("Tasks");
            modelBuilder.Entity<TaskItem>().HasKey(t => t.Id);
            modelBuilder.Entity<TaskItem>().Property(t => t.Title).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<TaskItem>().Property(t => t.DueDate).IsRequired();
            modelBuilder.Entity<TaskItem>().Property(t => t.Priority).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<TaskItem>().Property(t => t.IsCompleted).IsRequired();
            modelBuilder.Entity<TaskItem>().Property(t => t.SubjectCode).HasMaxLength(50);
            modelBuilder.Entity<TaskItem>().Property(t => t.SubjectName).HasMaxLength(100);
            modelBuilder.Entity<TaskItem>().Property(t => t.Description).HasMaxLength(1000);

            // Configure ScheduleItem
            modelBuilder.Entity<ScheduleItem>().ToTable("Schedule");
            modelBuilder.Entity<ScheduleItem>().HasKey(s => s.Id);
            modelBuilder.Entity<ScheduleItem>().Property(s => s.DayOfWeek).IsRequired();
            modelBuilder.Entity<ScheduleItem>().Property(s => s.SubjectCode).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<ScheduleItem>().Property(s => s.SubjectName).HasMaxLength(100);
            modelBuilder.Entity<ScheduleItem>().Property(s => s.StartTime).IsRequired().HasMaxLength(5);
            modelBuilder.Entity<ScheduleItem>().Property(s => s.EndTime).IsRequired().HasMaxLength(5);
            modelBuilder.Entity<ScheduleItem>().Property(s => s.Location).HasMaxLength(100);
            modelBuilder.Entity<ScheduleItem>().Property(s => s.Instructor).HasMaxLength(100);

            // Configure TodoItem
            modelBuilder.Entity<TodoItem>().ToTable("Todos");
            modelBuilder.Entity<TodoItem>().HasKey(t => t.Id);
            modelBuilder.Entity<TodoItem>().Property(t => t.Title).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<TodoItem>().Property(t => t.IsCompleted).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(t => t.Category).HasMaxLength(50);
            modelBuilder.Entity<TodoItem>().Property(t => t.CreatedDate).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(t => t.ModifiedDate);

            base.OnModelCreating(modelBuilder);
        }
    }
}

