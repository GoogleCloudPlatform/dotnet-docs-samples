using System;
using System.IO;
using System.Text.RegularExpressions;

namespace grs_events
{
    public static class UpdateUserEventsJson
    {
        private const string FileName = "user_events.json";
        private const string InvalidFileName = "user_events_some_invalid.json";

        private static readonly string FilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), $"grs-events/resources/{FileName}");
        private static readonly string InvalidFilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), $"grs-events/resources/{InvalidFileName}");

        // Get the yesterday's date
        private static readonly string RequestTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1).ToString("s");

        // Update events timesptamp
        private static void UpdateEventsTimeStamp(string jsonFile)
        {
            string text = File.ReadAllText(jsonFile);

            var updatedText = Regex.Replace(text, "\"eventTime\":\"([0-9]{4})-([0-9]{2})-([0-9]{2})", "\"eventTime\":\"" + RequestTimeStamp);

            using StreamWriter file = new(jsonFile, append: false);
            file.Write(updatedText);
        }

        // Perform update events timestamp
        [Attributes.Example]
        public static void PerformUpdateEventsTimeStamp()
        {
            UpdateEventsTimeStamp(FilePath);
            UpdateEventsTimeStamp(InvalidFilePath);
        }
    }
}