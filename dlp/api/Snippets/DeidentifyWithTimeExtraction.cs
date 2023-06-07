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

// [START dlp_deidentify_time_extract]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyWithTimeExtraction
{
    public static Table Deidentify(
        string projectId,
        Table tableToInspect = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value
                {
                    DateValue = new Google.Type.Date { Day = 10, Month = 6, Year = 2020 }
                }
            };
            var row2 = new Value[]
            {
                new Value
                {
                    DateValue = new Google.Type.Date { Day = 23, Month = 12, Year = 2022 }
                }
            };
            var row3 = new Value[]
            {
                new Value
                {
                    DateValue = new Google.Type.Date { Day = 1, Month = 1, Year = 2023 }
                }
            };

            tableToInspect = new Table
            {
                Headers = { new FieldId { Name = "Date" } },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } },
                    new Table.Types.Row { Values = { row3 } }
                }
            };
        }

        // Construct the table content item.
        var contentItem = new ContentItem { Table = tableToInspect };

        // Construct the time part config to deidentify specific value from the date.
        var timePartConfig = new TimePartConfig
        {
            PartToExtract = TimePartConfig.Types.TimePart.Year
        };

        // Construct the deidentify config using time part config created above.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            TimePartConfig = timePartConfig
                        }
                    }
                }
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifyConfig,
            Item = contentItem
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Inspect the response.
        Console.Write(response.Item.Table);

        return response.Item.Table;
    }
}
// [END dlp_deidentify_time_extract]
