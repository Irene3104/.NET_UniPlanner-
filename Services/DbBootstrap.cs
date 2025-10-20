using System;
using System.IO;
using UniPlanner.Data;

namespace UniPlanner.Services
{
    /// <summary>
    /// Database initialization and schema creation
    /// </summary>
    public static class DbBootstrap
    {
        /// <summary>
        /// Ensure database and tables are created
        /// </summary>
        public static void EnsureCreated()
        {
            try
            {
                // Get database path and ensure directory exists
                var dbPath = GetDatabasePath();
                var directory = Path.GetDirectoryName(dbPath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Use EF Core to ensure database is created
                using (var context = new UniPlannerContext())
                {
                    context.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Get full database file path
        /// </summary>
        private static string GetDatabasePath()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            var dbPath = Path.Combine(projectRoot, "Data", "uni.db");
            return dbPath;
        }

        /// <summary>
        /// Check if database exists
        /// </summary>
        public static bool DatabaseExists()
        {
            return File.Exists(GetDatabasePath());
        }

        /// <summary>
        /// Delete database file (for testing/reset)
        /// </summary>
        public static void DeleteDatabase()
        {
            var path = GetDatabasePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

