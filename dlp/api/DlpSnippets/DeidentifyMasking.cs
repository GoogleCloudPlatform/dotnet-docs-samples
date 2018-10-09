// Copyright (c) 2018 Google LLC.
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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;

partial class DlpSnippets
{
    public string DeidentiyMasking(string projectId = "YOUR-PROJECT-ID")
    {
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
            ParentAsProjectName = new ProjectName(projectId),
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
            Item = new ContentItem { Value = "My SSN is 372819127." }
        };

        DlpServiceClient dlp = DlpServiceClient.Create();
        var response = dlp.DeidentifyContent(request);

        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response.Item.Value;
    }
}

// [END dlp_deidentify_masking]
