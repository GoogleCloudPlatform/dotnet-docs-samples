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

// [START dlp_quickstart]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class ClientLibraryQuickStart
{
    public static InspectContentResponse QuickStart(string projectId, string text, IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct the byte content item.
        var byteContentItem = new ByteContentItem
        {
            Type = ByteContentItem.Types.BytesType.TextUtf8,
            Data = Google.Protobuf.ByteString.CopyFromUtf8(text)
        };

        // Construct content item by setting byte item.
        var contentItem = new ContentItem { ByteItem = byteContentItem };

        // Set minimum likelihood and limits.
        var minLikelihood = Likelihood.Possible;
        var limits = new InspectConfig.Types.FindingLimits
        {
            MaxFindingsPerRequest = 0
        };

        // Constuct a inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "PERSON_NAME" }, new InfoType { Name = "US_STATE" } } },
            MinLikelihood = minLikelihood,
            Limits = limits,
            IncludeQuote = true,
        };

        // Construct a request config.
        var request = new InspectContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            Item = contentItem,
            InspectConfig = inspectConfig
        };

        // Call the API.
        var response = dlp.InspectContent(request);

        // Inspect the response.
        var resultFindings = response.Result.Findings;

        if (resultFindings.Count < 0)
        {
            Console.WriteLine("No Findings.");
            return null;
        };

        Console.WriteLine($"Findings: {resultFindings.Count}");
        foreach (var f in resultFindings)
        {
            Console.WriteLine("Quote: " + f.Quote);
            Console.WriteLine("Info type: " + f.InfoType.Name);
            Console.WriteLine("Likelihood: " + f.Likelihood);
        }
        return response;
    }
}

// [END dlp_quickstart]
