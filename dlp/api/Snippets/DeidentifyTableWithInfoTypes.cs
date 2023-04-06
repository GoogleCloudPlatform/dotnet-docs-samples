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

// [START dlp_deidentify_table_infotypes]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyTableWithInfoTypes
{
    public static Table DeidentifyTable(
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

        // Construct Replace With InfoTypes config to replace the match.
        var replaceInfoTypesConfig = new ReplaceWithInfoTypeConfig();

        // Construct Fields to be de-identified.
        var fieldIds = new FieldId[] { new FieldId { Name = "PATIENT" }, new FieldId { Name = "FACTOID" } };

        // Construct InfoType Transformation.
        var infoTypeTransformations = new InfoTypeTransformations
        {
            Transformations =
            {
                new InfoTypeTransformations.Types.InfoTypeTransformation
                {
                    PrimitiveTransformation = new PrimitiveTransformation
                    {
                        ReplaceWithInfoTypeConfig = replaceInfoTypesConfig
                    },
                    InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "PERSON_NAME" } } }
                }
            }
        };

        // Construct the de-identify config using replace config.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        InfoTypeTransformations = infoTypeTransformations,
                        Fields = { fieldIds }
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

// [END dlp_deidentify_table_infotypes]
