// Copyright 2020 Google Inc.
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

// [START dlp_redact_image_listed_infotypes]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.IO;

public class RedactImageWithListedInfotypes
{
    public static RedactImageResponse Redact(string projectId, string originalImagePath, string redactedImagePath)
    {
        var request = new RedactImageRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            InspectConfig = new InspectConfig
            {
                MinLikelihood = Likelihood.Likely,
                Limits = new InspectConfig.Types.FindingLimits() { MaxFindingsPerItem = 5 },
                IncludeQuote = true,
                InfoTypes =
                    {
                        new InfoType { Name = "PHONE_NUMBER" },
                        new InfoType { Name = "EMAIL_ADDRESS" }
                    }
            },
            ByteItem = new ByteContentItem
            {
                Type = ByteContentItem.Types.BytesType.ImagePng,
                Data = ByteString.FromStream(new FileStream(originalImagePath, FileMode.Open))
            },
        };

        var client = DlpServiceClient.Create();
        var response = client.RedactImage(request);

        Console.WriteLine($"Extracted text: {response.ExtractedText}");

        // Writes redacted image into file
        response.RedactedImage.WriteTo(new FileStream(redactedImagePath, FileMode.Create, FileAccess.Write));

        return response;
    }
}

// [END dlp_redact_image_listed_infotypes]
