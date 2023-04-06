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

// [START dlp_deidentify_replace]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyWithReplacement
{
    public static DeidentifyContentResponse Deidentify(
        string projectId,
        string text,
        string replaceText = null,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Set the info type if null.
        var infotypes = infoTypes ?? new InfoType[] { new InfoType { Name = "EMAIL_ADDRESS" } };

        // Construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infotypes }
        };

        // Construct the replace value config.
        var replaceConfig = new ReplaceValueConfig
        {
            NewValue = new Value { StringValue = replaceText ?? "[email-address]" }
        };

        // Construct the deidentify config using replace value config.
        var deidentifyConfig = new DeidentifyConfig
        {
            InfoTypeTransformations = new InfoTypeTransformations
            {
                Transformations =
                {
                    new InfoTypeTransformations.Types.InfoTypeTransformation
                    {
                        InfoTypes = { infotypes },
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            ReplaceConfig = replaceConfig
                        }
                    }
                },
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifyConfig,
            InspectConfig = inspectConfig,
            Item = new ContentItem { Value = text }
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Check the deidentified content.
        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_deidentify_replace]
