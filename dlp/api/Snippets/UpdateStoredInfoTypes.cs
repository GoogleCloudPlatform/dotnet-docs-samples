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

// [START dlp_update_stored_infotype]

using System;
using Google.Cloud.Dlp.V2;
using Google.Protobuf.WellKnownTypes;

public class UpdateStoredInfoTypes
{
    public static StoredInfoType Update(
        string gcsFileUri,
        string storedInfoTypePath,
        string outputPath)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Construct the stored infotype config. Here, we will change the source from bigquery table to GCS file.
        var storedConfig = new StoredInfoTypeConfig
        {
            LargeCustomDictionary = new LargeCustomDictionaryConfig
            {
                CloudStorageFileSet = new CloudStorageFileSet
                {
                    Url = gcsFileUri
                },
                OutputPath = new CloudStoragePath
                {
                    Path = outputPath
                }
            }
        };

        // Construct the request using the stored config by specifying the update mask object
        // which represent the path of field to be updated.
        var request = new UpdateStoredInfoTypeRequest
        {
            Config = storedConfig,
            Name = storedInfoTypePath,
            UpdateMask = new FieldMask
            {
                Paths =
                {
                    "large_custom_dictionary.cloud_storage_file_set.url"
                }
            }
        };

        // Call the API.
        StoredInfoType response = dlp.UpdateStoredInfoType(request);

        // Inspect the result.
        Console.WriteLine(response);
        return response;
    }
}

// [END dlp_update_stored_infotype]
