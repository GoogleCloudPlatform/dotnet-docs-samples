// Copyright (c) 2023 Google LLC.
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

// [START dlp_inspect_augment_infotypes]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Linq;
using static Google.Cloud.Dlp.V2.CustomInfoType.Types;

public class InspectDataUsingAugmentInfoTypes
{
    public static InspectContentResponse InspectData(
        string projectId,
        string text,
        InfoType infoType = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Specify the type of info to be inspected and construct the infotype.
        var infotype = infoType ?? new InfoType { Name = "PERSON_NAME" };

        // Construct the custom infoTypes with dictionary.
        var customInfoTypes = new CustomInfoType
        {
            InfoType = infotype,
            Dictionary = new Dictionary
            {
                WordList = new Dictionary.Types.WordList
                {
                    Words = { new string[] { "quasimodo" } }
                }
            }
        };

        // Construct the inspect config using custom infoTypes.
        var inspectConfig = new InspectConfig
        {
            CustomInfoTypes = { customInfoTypes },
            IncludeQuote = true,
            InfoTypes = { infotype }
        };

        // Construct the request.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectConfig = inspectConfig,
            Item = new ContentItem
            {
                ByteItem = new ByteContentItem
                {
                    Data = ByteString.CopyFromUtf8(text),
                    Type = ByteContentItem.Types.BytesType.TextUtf8
                }
            }
        };

        // Call the API.
        InspectContentResponse response = dlp.InspectContent(request);

        // Parse the response.
        var findings = response.Result.Findings;
        Console.WriteLine($"Finding: {findings.Count}");

        foreach (var f in findings)
        {
            Console.WriteLine("\tQuote: " + f.Quote);
            Console.WriteLine("\tInfo type: " + f.InfoType.Name);
            Console.WriteLine("\tLikelihood: " + f.Likelihood);
        }

        return response;
    }
}

// [END dlp_inspect_augment_infotypes]
