// Copyright 2023 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START dlp_inspect_table]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class InspectTable
{
    public static InspectContentResponse InspectTableData(
        string projectId,
        Table tableToInspect = null,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value { StringValue = "John Doe" },
                new Value { StringValue = "(206) 555-0123" }
            };
            var row2 = new Value[]
            {
                new Value { StringValue = "Mark Twain" },
                new Value { StringValue = "(450) 555-0123" }
            };

            tableToInspect = new Table
            {
                Headers =
                {
                    new FieldId { Name = "Name" }, new FieldId { Name = "Phone" }
                },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } }
                }
            };
        }

        // Set content item.
        var contentItem = new ContentItem { Table = tableToInspect };

        // Construct inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes =
            {
                infoTypes ?? new InfoType[] { new InfoType { Name = "PHONE_NUMBER" } }
            },
            IncludeQuote = true,
        };

        // Construct a request.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectConfig = inspectConfig,
            Item = contentItem,
        };

        // Call the API.
        var response = dlp.InspectContent(request);

        // Inspect the results.
        var resultFindings = response.Result.Findings;

        Console.WriteLine($"Findings: {resultFindings.Count}");

        foreach (var f in resultFindings)
        {
            Console.WriteLine("Quote: " + f.Quote);
            Console.WriteLine("Info type: " + f.InfoType.Name);
            Console.WriteLine("Likelihood: " + f.Likelihood);
        }

        return response;
    }
}

// [END dlp_inspect_table]
