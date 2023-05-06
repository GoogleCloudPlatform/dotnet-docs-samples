// Copyright 2023 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START dlp_redact_image_all_infotypes]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.IO;

public class RedactSensitiveDataFromImageUsingDefaultInfoTypes
{
    public static RedactImageResponse RedactImage(
        string projectId,
        string originalImagePath,
        string redactedImagePath)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the content item.
        var byteContentItem = new ByteContentItem
        {
            Type = ByteContentItem.Types.BytesType.ImagePng,
            Data = ByteString.FromStream(new FileStream(originalImagePath, FileMode.Open))
        };

        // Construct the Redact request to be sent by the client. Do not specify the type of info to redact.
        var request = new RedactImageRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            ByteItem = byteContentItem
        };

        // Call the API.
        var response = dlp.RedactImage(request);

        // Inspect the response.
        Console.WriteLine($"Redacted image written to: {redactedImagePath}");

        // Writes redacted image into file
        response.RedactedImage.WriteTo(new FileStream(redactedImagePath, FileMode.Create, FileAccess.Write));

        return response;
    }
}

// [END dlp_redact_image_all_infotypes]
