// Copyright 2022 Google Inc.

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

using System.IO;

public static class EventsCreateBigQueryTable
{
    private const string EventsFileName = "user_events.json";
    private const string InvalidEventsFileName = "user_events_some_invalid.json";
    private const string EventsDataSet = "user_events";
    private const string EventsTable = "events";
    private const string InvalidEventsTable = "events_some_invalid";
    private const string EventsSchema = "events_schema.json";

    private static readonly string eventsFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{EventsFileName}");
    private static readonly string invalidEventsFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{InvalidEventsFileName}");
    private static readonly string eventsSchemaFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{EventsSchema}");

    /// <summary>
    /// Create events BigQuery tables with data.
    /// </summary>
    [Runner.Attributes.Example]
    public static void PerformCreationOfBigQueryTable()
    {
        // Create a BigQuery tables with data.
        CreateTestResources.CreateBQDataSet(EventsDataSet);
        CreateTestResources.CreateAndPopulateBQTable(EventsDataSet, EventsTable, eventsSchemaFilePath, eventsFilePath);
        CreateTestResources.CreateAndPopulateBQTable(EventsDataSet, InvalidEventsTable, eventsSchemaFilePath, invalidEventsFilePath);
    }
}
