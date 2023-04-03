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

// [START dlp_inspect_custom_regex]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class InspectDataWithCustomRegex
{
    public static InspectContentResponse InspectDataCustomRegex(
        string projectId,
        string text,
        string customRegex,
        InfoType infoType = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct content item by setting the text.
        var contentItem = new ContentItem { Value = text };

        // Construct the custom regex detector.
        var customInfoType = new CustomInfoType
        {
            InfoType = infoType ?? new InfoType { Name = "C_MRN" },
            Regex = new CustomInfoType.Types.Regex { Pattern = customRegex }
        };

        // Construct Inspect Config.
        var inspectConfig = new InspectConfig
        {
            CustomInfoTypes = { customInfoType },
            IncludeQuote = true,
            MinLikelihood = Likelihood.Possible
        };

        // Construct the request.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            Item = contentItem,
            InspectConfig = inspectConfig,
        };

        // Call the API.
        var response = dlp.InspectContent(request);

        // Inspect the results.
        var resultFindings = response.Result.Findings;

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

// [END dlp_inspect_custom_regex]
