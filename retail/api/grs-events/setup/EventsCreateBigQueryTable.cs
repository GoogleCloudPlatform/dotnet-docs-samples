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
using System.Diagnostics;

namespace grs_events.setup
{
    public static class EventsCreateBigQueryTable
    {
        private static readonly string ProjectId = Environment.GetEnvironmentVariable("PROJECT_ID");

        private const string DataSetId = "user_events";
        private const string ValidEventsTableId = "events";
        private const string InvalidEventsTableId = "events_some_invalid";
        private const string EventsSchema = "resources/events_schema.json";
        private const string ValidEventsSourceFile = "resources/user_events.json";
        private const string InvalidEventsSourceFile = "resources/user_events_some_invalid.json";

        private const string WindowsTerminalVarName = "ComSpec";
        private const string UnixTerminalVarName = "SHELL";
        private const string WindowsTerminalPrefix = "/c ";
        private const string UnixTerminalPrefix = "-c ";
        private static readonly string CurrentOperatingSystemName = Environment.OSVersion.VersionString;
        private static readonly string CurrentTerminalVarName = CurrentOperatingSystemName.Contains("Windows") ? WindowsTerminalVarName : UnixTerminalVarName;
        private static readonly string CurrentTerminalPrefix = CurrentOperatingSystemName.Contains("Windows") ? WindowsTerminalPrefix : UnixTerminalPrefix;
        private static readonly string CurrentTerminalFile = Environment.GetEnvironmentVariable(CurrentTerminalVarName);

        private static void CreateBQDataSet(string dataSetName)
        {
            var listDataSets = ListBQDataSets();
            if (!listDataSets.Contains(dataSetName))
            {
                string createDataSetCommand = CurrentTerminalPrefix + $"bq --location=US mk -d --default_table_expiration 3600 --description \"This is my dataset.\" {ProjectId}:{dataSetName}";
                string consoleOutput = string.Empty;

                var processStartInfo = new ProcessStartInfo(CurrentTerminalFile, createDataSetCommand)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process())
                {
                    process.StartInfo = processStartInfo;

                    process.Start();

                    consoleOutput = process.StandardOutput.ReadToEnd();
                }
            }
            else
            {
                Console.WriteLine($"Dataset {dataSetName} already exists.");
            }
        }

        private static string ListBQDataSets()
        {
            string dataSets = string.Empty;

            string listDataSetCommand = CurrentTerminalPrefix + $"bq ls --project_id {ProjectId}";

            var processStartInfo = new ProcessStartInfo(CurrentTerminalFile, listDataSetCommand)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.Start();

                dataSets = process.StandardOutput.ReadToEnd();

                Console.WriteLine(dataSets);
            }

            return dataSets;
        }

        private static void CreateBQTable(string dataSet, string tableName, string schema)
        {
            var listBQTables = ListBQTables(dataSet);
            if (!listBQTables.Contains(dataSet))
            {
                string consoleOutput = string.Empty;

                var createTableCommand = CurrentTerminalPrefix + $"bq mk --table {ProjectId}:{dataSet}.{tableName} {schema}";

                var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, createTableCommand)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = procStartInfo;
                    process.Start();

                    consoleOutput = process.StandardOutput.ReadToEnd();
                    Console.WriteLine(consoleOutput);
                }
            }
            else
            {
                Console.WriteLine($"Table {tableName} already exists.");
            }

        }

        private static string ListBQTables(string dataSet)
        {
            string tables = string.Empty;
            var listTablesCommand = CurrentTerminalPrefix + $"bq ls {ProjectId}:{dataSet}";

            var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, listTablesCommand)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = procStartInfo;
                process.Start();

                tables = process.StandardOutput.ReadToEnd();
                Console.WriteLine("Tables: \n" + tables);

                return tables;
            }
        }

        private static void UploadDataToBQTable(string dataSet, string tableName, string source, string schema)
        {
            string consoleOutput = string.Empty;

            var uploadDataCommand = CurrentTerminalPrefix + $"bq load --source_format=NEWLINE_DELIMITED_JSON {ProjectId}:{dataSet}.{tableName} {source} {schema}";

            var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, uploadDataCommand)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = procStartInfo;
                process.Start();

                consoleOutput = process.StandardOutput.ReadToEnd();
                Console.WriteLine(consoleOutput);
            }
        }

        [Attributes.Example]
        public static void PerformCreationOfBigQueryTable()
        {
            CreateBQDataSet(DataSetId);
            CreateBQTable(DataSetId, ValidEventsTableId, EventsSchema);
            UploadDataToBQTable(DataSetId, ValidEventsTableId, ValidEventsSourceFile, EventsSchema);
            CreateBQTable(DataSetId, InvalidEventsTableId, EventsSchema);
            UploadDataToBQTable(DataSetId, InvalidEventsTableId, InvalidEventsSourceFile, EventsSchema);
        }
    }
}