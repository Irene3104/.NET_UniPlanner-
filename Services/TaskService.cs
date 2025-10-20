using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using UniPlanner.Models;

namespace UniPlanner.Services
{
    /// <summary>
    /// Task management service with SQLite + Dapper
    /// </summary>
    public class TaskService : IRepository<TaskItem>
    {
        private string GetConnectionString()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            var dbPath = Path.Combine(projectRoot, "Data", "uni.db");
            return $"Data Source={dbPath};Foreign Keys=True;";
        }

        /// <summary>
        /// Add new task to database
        /// </summary>
        public void Add(TaskItem item)
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                conn.Execute(
                    @"INSERT INTO Tasks(Title, DueDate, Priority, IsCompleted, Subject, SubjectName, Description) 
                      VALUES(@Title, @DueDate, @Priority, @IsCompleted, @SubjectCode, @SubjectName, @Description)",
                    new
                    {
                        item.Title,
                        DueDate = item.DueDate.ToString("dd-MM-yyyy"),
                        item.Priority,
                        IsCompleted = item.IsCompleted ? 1 : 0,
                        SubjectCode = item.SubjectCode,
                        item.SubjectName,
                        item.Description
                    }
                );
            }
        }

        /// <summary>
        /// Update existing task
        /// </summary>
        public void Update(TaskItem item)
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                conn.Execute(
                    @"UPDATE Tasks 
                      SET Title = @Title, DueDate = @DueDate, Priority = @Priority, 
                          IsCompleted = @IsCompleted, Subject = @SubjectCode, SubjectName = @SubjectName, Description = @Description
                      WHERE Id = @Id",
                    new
                    {
                        item.Id,
                        item.Title,
                        DueDate = item.DueDate.ToString("dd-MM-yyyy"),
                        item.Priority,
                        IsCompleted = item.IsCompleted ? 1 : 0,
                        SubjectCode = item.SubjectCode,
                        item.SubjectName,
                        item.Description
                    }
                );
            }
        }

        /// <summary>
        /// Delete task by ID
        /// </summary>
        public void Delete(int id)
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                conn.Execute("DELETE FROM Tasks WHERE Id = @Id", new { Id = id });
            }
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        public TaskItem? GetById(int id)
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SELECT * FROM Tasks WHERE Id = @Id", 
                    new { Id = id }
                );
                
                return result != null ? MapToTaskItem(result) : null;
            }
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        public IReadOnlyList<TaskItem> GetAll()
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                var results = conn.Query<dynamic>("SELECT * FROM Tasks");
                return results.Select(MapToTaskItem).ToList();
            }
        }

        /// <summary>
        /// Mark task as completed
        /// </summary>
        public void MarkComplete(int id)
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                conn.Execute("UPDATE Tasks SET IsCompleted = 1 WHERE Id = @Id", new { Id = id });
            }
        }

        /// <summary>
        /// Toggle task completion status
        /// </summary>
        public void ToggleComplete(int id)
        {
            using (var conn = new SqliteConnection(GetConnectionString()))
            {
                conn.Execute(@"UPDATE Tasks 
                              SET IsCompleted = CASE WHEN IsCompleted = 1 THEN 0 ELSE 1 END 
                              WHERE Id = @Id", new { Id = id });
            }
        }

        /// <summary>
        /// Get upcoming tasks ordered by due date (LINQ demonstration)
        /// </summary>
        public IReadOnlyList<TaskItem> GetUpcomingOrdered()
        {
            return GetAll()
                .Where(t => t.DueDate >= DateTime.Today && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority == "High" ? 1 : t.Priority == "Medium" ? 2 : 3)
                .ToList();
        }

        /// <summary>
        /// Get overdue tasks (LINQ demonstration)
        /// </summary>
        public IReadOnlyList<TaskItem> GetOverdue()
        {
            return GetAll()
                .Where(t => t.IsOverdue())
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        /// <summary>
        /// Get completed tasks (LINQ demonstration)
        /// </summary>
        public IReadOnlyList<TaskItem> GetCompleted()
        {
            return GetAll()
                .Where(t => t.IsCompleted)
                .OrderByDescending(t => t.DueDate)
                .ToList();
        }

        /// <summary>
        /// Get tasks by priority (LINQ demonstration)
        /// </summary>
        public IReadOnlyList<TaskItem> GetByPriority(string priority)
        {
            return GetAll()
                .Where(t => t.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        /// <summary>
        /// Get tasks due this week (LINQ demonstration)
        /// </summary>
        public IReadOnlyList<TaskItem> GetThisWeek()
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            return GetAll()
                .Where(t => t.DueDate >= startOfWeek && t.DueDate < endOfWeek && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        /// <summary>
        /// Map dynamic result to TaskItem
        /// </summary>
        private TaskItem MapToTaskItem(dynamic row)
        {
            return new TaskItem
            {
                Id = (int)(long)row.Id,
                Title = row.Title,
                DueDate = DateTime.Parse(row.DueDate),
                Priority = row.Priority,
                IsCompleted = row.IsCompleted == 1,
                SubjectCode = row.Subject,
                SubjectName = HasProperty(row, "SubjectName") ? row.SubjectName : null,
                Description = row.Description
            };
        }

        private bool HasProperty(dynamic obj, string propertyName)
        {
            try
            {
                var value = ((IDictionary<string, object>)obj).ContainsKey(propertyName);
                return value;
            }
            catch
            {
                return false;
            }
        }
    }
}

