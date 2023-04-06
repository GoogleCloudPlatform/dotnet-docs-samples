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

// [START dlp_deidentify_table_row_suppress]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyTableWithRowSuppress
{
    public static Table DeidentifyTable(
        string projectId,
        Table tableToInspect = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value { StringValue = "101" },
                new Value { StringValue = "Charles Dickens" },
                new Value { StringValue = "95" }
            };
            var row2 = new Value[]
            {
                new Value { StringValue = "22" },
                new Value { StringValue = "Jane Austin" },
                new Value { StringValue = "21" }
            };
            var row3 = new Value[]
            {
                new Value { StringValue = "55" },
                new Value { StringValue = "Mark Twain" },
                new Value { StringValue = "75" }
            };

            tableToInspect = new Table
            {
                Headers =
                {
                    new FieldId { Name = "AGE" },
                    new FieldId { Name = "PATIENT" },
                    new FieldId { Name = "HAPPINESS SCORE" }
                },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } },
                    new Table.Types.Row { Values = { row3 } }
                }
            };
        }

        // Construct the byte content item.
        var contentItem = new ContentItem { Table = tableToInspect };

        // Construct the conditions.
        var conditions = new RecordCondition.Types.Conditions
        {
            Conditions_ =
            {
                new RecordCondition.Types.Condition
                {
                    Field = new FieldId { Name = "AGE" },
                    Operator = RelationalOperator.GreaterThan,
                    Value = new Value { IntegerValue = 89 }
                }
            }
        };

        // Construct the deidentify config using the record suppression and conditions.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                RecordSuppressions =
                {
                    new RecordSuppression
                    {
                        Condition = new RecordCondition
                        {
                            Expressions = new RecordCondition.Types.Expressions
                            {
                                Conditions = conditions
                            }
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
        Console.WriteLine(response.Item.Table);
        return response.Item.Table;
    }
}

// [END dlp_deidentify_table_row_suppress]
