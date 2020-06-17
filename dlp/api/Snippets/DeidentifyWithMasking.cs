// Copyright (c) 2020 Google LLC.
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

// [START dlp_deidentify_masking]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyWithMasking
{
    public static DeidentifyContentResponse Deidentify(string projectId, string text)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct a request.
        var transformation = new InfoTypeTransformations.Types.InfoTypeTransformation
        {
            PrimitiveTransformation = new PrimitiveTransformation
            {
                CharacterMaskConfig = new CharacterMaskConfig
                {
                    MaskingCharacter = "*",
                    NumberToMask = 5,
                    ReverseOrder = false,
                }
            }
        };
        var request = new DeidentifyContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            InspectConfig = new InspectConfig
            {
                InfoTypes =
                {
                    new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" }
                }
            },
            DeidentifyConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations
                {
                    Transformations = { transformation }
                }
            },
            Item = new ContentItem { Value = text }
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Inspect the results.
        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_deidentify_masking]
