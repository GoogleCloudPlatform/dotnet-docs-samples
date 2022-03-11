// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Class for updating user events timestamps from json.
/// </summary>
public static class UpdateUserEventsJson
{
    private const string FileName = "user_events.json";
    private const string InvalidFileName = "user_events_some_invalid.json";

    private static readonly string FilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailEvents.Samples/resources/{FileName}");
    private static readonly string InvalidFilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailEvents.Samples/resources/{InvalidFileName}");

    // Get the yesterday's date
    private static readonly string RequestTimeStamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");

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
        string text = File.ReadAllText(jsonFile);

        var updatedText = Regex.Replace(text, "\"eventTime\":\"([0-9]{4})-([0-9]{2})-([0-9]{2})", "\"eventTime\":\"" + RequestTimeStamp);

        using StreamWriter file = new StreamWriter(jsonFile, append: false);
        file.Write(updatedText);
    }

    /// <summary>
    /// Perform update events timestamp.
    /// </summary>
    [Runner.Attributes.Example]
    public static void PerformUpdateEventsTimeStamp()
    {
        UpdateEventsTimeStamp(FilePath);
        UpdateEventsTimeStamp(InvalidFilePath);
    }
}
