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

// [START dlp_redact_image_colored_infotypes]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.IO;

public class RedactImageWithColorCodedInfoTypes
{
    public static RedactImageResponse RedactImage(
        string projectId,
        string originalImagePath,
        string redactedImagePath)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the content item by providing the image and its type.
        var byteContentItem = new ByteContentItem
        {
            Type = ByteContentItem.Types.BytesType.ImagePng,
            Data = ByteString.FromStream(new FileStream(originalImagePath, FileMode.Open))
        };

        // Define types of info and associate each one with a different color to redact config.
        var ssnRedactionConfig = new RedactImageRequest.Types.ImageRedactionConfig
        {
            InfoType = new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" },
            RedactionColor = new Color
            {
                Red = 0.3f,
                Green = 0.1f,
                Blue = 0.6f
            }
        };

        var emailRedactionConfig = new RedactImageRequest.Types.ImageRedactionConfig
        {
            InfoType = new InfoType { Name = "EMAIL_ADDRESS" },
            RedactionColor = new Color
            {
                Red = 0.5f,
                Green = 0.5f,
                Blue = 1f
            }
        };

        var phoneRedactionConfig = new RedactImageRequest.Types.ImageRedactionConfig
        {
            InfoType = new InfoType { Name = "PHONE_NUMBER" },
            RedactionColor = new Color
            {
                Red = 1f,
                Green = 0f,
                Blue = 0.6f
            }
        };

        // Construct the Redact request to be sent by the client. Do not specify the type of info to redact.
        var request = new RedactImageRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            ImageRedactionConfigs = { ssnRedactionConfig, emailRedactionConfig, phoneRedactionConfig },
            ByteItem = byteContentItem
        };

        // Call the API.
        RedactImageResponse response = dlp.RedactImage(request);

        // Writes redacted image into file
        response.RedactedImage.WriteTo(new FileStream(redactedImagePath, FileMode.Create, FileAccess.Write));

        Console.WriteLine($"Redacted image written to: {redactedImagePath}");

        return response;
    }
}

// [END dlp_redact_image_colored_infotypes]
