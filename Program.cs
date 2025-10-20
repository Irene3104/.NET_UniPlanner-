using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using UniPlanner.Forms;
using UniPlanner.Services;

namespace UniPlanner
{
    /// <summary>
    /// Application entry point
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Enable visual styles for modern UI
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Configure global culture (Sydney: dd-MM-yyyy) - Australian English
                var culture = new CultureInfo("en-AU");
                culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
                culture.DateTimeFormat.LongDatePattern = "dddd, dd MMMM yyyy";
                culture.DateTimeFormat.FullDateTimePattern = "dddd, dd MMMM yyyy HH:mm";
                culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
                
                // Set English day names
                culture.DateTimeFormat.AbbreviatedDayNames = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                culture.DateTimeFormat.DayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                culture.DateTimeFormat.ShortestDayNames = new[] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
                
                // Set English month names
                culture.DateTimeFormat.MonthNames = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "" };
                culture.DateTimeFormat.AbbreviatedMonthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "" };
                culture.DateTimeFormat.MonthGenitiveNames = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "" };
                culture.DateTimeFormat.AbbreviatedMonthGenitiveNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "" };

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                // Configure data directory to project root Data folder
                var projectDataDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data"));
                if (!System.IO.Directory.Exists(projectDataDir))
                {
                    System.IO.Directory.CreateDirectory(projectDataDir);
                }
                AppDomain.CurrentDomain.SetData("DataDirectory", projectDataDir);

                // Initialize database
                DbBootstrap.EnsureCreated();

                // Run main form
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                string errorMessage = $"Application startup error:\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nDetails: {ex.InnerException.Message}";
                }
                errorMessage += "\n\nPlease contact support.";

                MessageBox.Show(
                    errorMessage,
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
