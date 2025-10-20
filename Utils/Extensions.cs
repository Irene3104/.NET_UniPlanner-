using System;

namespace UniPlanner.Utils
{
    /// <summary>
    /// Extension methods for common types
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get week number of the year
        /// </summary>
        public static int GetWeekNumber(this DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, 
                System.Globalization.CalendarWeekRule.FirstDay, 
                DayOfWeek.Sunday);
        }
    }
}

