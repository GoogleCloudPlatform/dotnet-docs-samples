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

// [START dlp_deidentify_table_primitive_bucketing]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyTableWithPrimitiveBucketing
{
    public static Table DeidentifyData(
        string projectId,
        Table tableToInspect = null)
    {
        // Instantiate dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value { IntegerValue = 1 },
                new Value { IntegerValue = 95 }
            };
            var row2 = new Value[]
            {
                new Value { IntegerValue = 2 },
                new Value { IntegerValue = 61 }
            };
            var row3 = new Value[]
            {
                new Value { IntegerValue = 3 },
                new Value { IntegerValue = 22 }
            };

            tableToInspect = new Table
            {
                Headers =
                {
                    new FieldId { Name = "user_id" },
                    new FieldId { Name = "score" }
                },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } },
                    new Table.Types.Row { Values = { row3 } }
                }
            };
        }

        // Specify the table and construct the content item.
        var contentItem = new ContentItem { Table = tableToInspect };

        // Specify how the content should be de-identified.
        var bucketingConfig = new BucketingConfig
        {
            Buckets =
            {
                new BucketingConfig.Types.Bucket
                {
                    Min = new Value { IntegerValue = 0 },
                    Max = new Value { IntegerValue = 25 },
                    ReplacementValue = new Value { StringValue = "Low" }
                },
                new BucketingConfig.Types.Bucket
                {
                    Min = new Value { IntegerValue = 25 },
                    Max = new Value { IntegerValue = 75 },
                    ReplacementValue = new Value { StringValue = "Medium" }
                },
                new BucketingConfig.Types.Bucket
                {
                    Min = new Value { IntegerValue = 75 },
                    Max = new Value { IntegerValue = 100 },
                    ReplacementValue = new Value { StringValue = "High" }
                }
            }
        };

        // Specify the fields to be encrypted.
        var fields = new FieldId[] { new FieldId { Name = "score" } };


        // Associate the de-identification with the specified field.
        var fieldTransformation = new FieldTransformation
        {
            PrimitiveTransformation = new PrimitiveTransformation
            {
                BucketingConfig = bucketingConfig
            },
            Fields = { fields }
        };

        // Construct the de-identify config.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations = { fieldTransformation }
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifyConfig,
            Item = contentItem,
        };

        // Call the API.
        DeidentifyContentResponse response = dlp.DeidentifyContent(request);

        // Inspect the response.
        Console.WriteLine(response.Item.Table);

        return response.Item.Table;
    }
}

// [END dlp_deidentify_table_primitive_bucketing]
