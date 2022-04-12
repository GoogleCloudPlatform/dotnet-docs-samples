using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RetailPredictions.Samples.setup
{
    public static class UpdateUserEventsJson
    {
        private const string FileName = "user_events.json";
        private static Random gen = new Random();

        private static readonly string FilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailPredictions.Samples/resources/{FileName}");

        /// <summary>
        /// Get the current solution directory full name.
        /// </summary>
        /// <param name="currentPath">The current path.</param>
        /// <returns>Full name of the current solution directory.</returns>
        private static string GetSolutionDirectoryFullName(string currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            return directory.FullName;
        }

        /// <summary>
        /// Update events timesptamp.
        /// </summary>
        /// <param name="jsonFile">The path to the json file with events.</param>
        private static void UpdateEventsTimeStamp(string jsonFile)
        {
            string[] alllines = File.ReadAllLines(jsonFile);

            for (int i = 0; i < alllines.Length; i++)
            {
                string date = GetRandomDate();
                alllines[i] = Regex.Replace(alllines[i], "\"eventTime\":\"(\\d{4}-\\d{2}-\\d{2}.*)\"", "\"eventTime\":\"" + date + "T10:27:42+00:00\"");
            }

            File.WriteAllLines(FilePath, alllines);
        }

        /// <summary>
        /// Get Random Date
        /// </summary>
        /// <returns></returns>
        private static string GetRandomDate()
        {
            DateTime start = DateTime.UtcNow.AddDays(-88);
            int range = (DateTime.UtcNow - start).Days;
            return start.AddDays(gen.Next(range)).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Perform update events timestamp.
        /// </summary>
        public static void UpdateEventsTimeStamp()
        {
            UpdateEventsTimeStamp(FilePath);
        }
    }

}
