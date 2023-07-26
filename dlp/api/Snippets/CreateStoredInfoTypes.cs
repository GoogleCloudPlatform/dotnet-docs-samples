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

// [START dlp_create_stored_infotype]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class CreateStoredInfoTypes
{
    public static StoredInfoType Create(
        string projectId,
        string outputPath,
        string storedInfoTypeId)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the stored infotype config by specifying the public table and 
        // cloud storage output path.
        var storedInfoTypeConfig = new StoredInfoTypeConfig
        {
            DisplayName = "Github Usernames",
            Description = "Dictionary of Github usernames used in commits.",
            LargeCustomDictionary = new LargeCustomDictionaryConfig
            {
                BigQueryField = new BigQueryField
                {
                    Table = new BigQueryTable
                    {
                        DatasetId = "samples",
                        ProjectId = "bigquery-public-data",
                        TableId = "github_nested"
                    },
                    Field = new FieldId
                    {
                        Name = "actor"
                    }
                },
                OutputPath = new CloudStoragePath
                {
                    Path = outputPath
                }
            },
        };

        // Construct the request.
        var request = new CreateStoredInfoTypeRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            Config = storedInfoTypeConfig,
            StoredInfoTypeId = storedInfoTypeId
        };

        // Call the API.
        StoredInfoType response = dlp.CreateStoredInfoType(request);

        // Inspect the response.
        Console.WriteLine($"Created the stored infotype at path: {response.Name}");

        return response;
    }
}

// [END dlp_create_stored_infotype]
