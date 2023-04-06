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

// [START dlp_deidentify_table_condition_masking]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyTableUsingMaskingAndLogic
{
    public static Table DeidentifyTable(
        string projectId,
        Table tableToInspect = null)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value { StringValue = "101" },
                new Value { StringValue = "Charles Dickens" },
                new Value { StringValue = "95" },
                new Value { StringValue = "Charles Dickens name was a curse invented by Shakespeare." }
            };
            var row2 = new Value[]
            {
                new Value { StringValue = "22" },
                new Value { StringValue = "Jane Austin" },
                new Value { StringValue = "21" },
                new Value { StringValue = "There are 14 kisses in Jane Austen's novels." }
            };
            var row3 = new Value[]
            {
                new Value { StringValue = "55" },
                new Value { StringValue = "Mark Twain" },
                new Value { StringValue = "75" },
                new Value { StringValue = "Mark Twain loved cats." }
            };

            tableToInspect = new Table
            {
                Headers =
                {
                    new FieldId { Name = "AGE" },
                    new FieldId { Name = "PATIENT" },
                    new FieldId { Name = "HAPPINESS SCORE" },
                    new FieldId { Name = "FACTOID" }
                },
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

        //Specify how the content should be de-identified.
        var primitiveTransformation = new PrimitiveTransformation
        {
            CharacterMaskConfig = new CharacterMaskConfig
            {
                MaskingCharacter = "*"
            }
        };

        // Specify the fields to be de-identified.
        var fields = new FieldId[] { new FieldId { Name = "HAPPINESS SCORE" } };

        // Specify when the above fields should be de-identified using condition.
        var condition = new RecordCondition.Types.Conditions
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

        // Apply the condition to records
        var recordCondition = new RecordCondition
        {
            Expressions = new RecordCondition.Types.Expressions
            {
                Conditions = condition
            }
        };

        // Associate the de-identification and conditions with the specified fields.
        var deidentifiedConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        PrimitiveTransformation = primitiveTransformation,
                        Fields = { fields },
                        Condition = recordCondition
                    }
                }
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifiedConfig,
            Item = contentItem
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Inspect the response.
        Console.WriteLine(response.Item.Table);
        return response.Item.Table;
    }
}
// [END dlp_deidentify_table_condition_masking]
