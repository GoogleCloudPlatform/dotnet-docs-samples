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

// [START dlp_inspect_phone_number]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class InspectPhoneNumber
{
    public static InspectContentResponse Inspect(
        string projectId,
        string text,
        Likelihood minLikelihood = Likelihood.Possible)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Set content item.
        var contentItem = new ContentItem { Value = text };

        // Construct inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { new InfoType { Name = "PHONE_NUMBER" } },
            IncludeQuote = true,
            MinLikelihood = minLikelihood
        };

        // Construct a request.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectConfig = inspectConfig,
            Item = contentItem,
        };

        // Call the API.
        var response = dlp.InspectContent(request);

        // Inspect the results.
        var resultFindings = response.Result.Findings;

        Console.WriteLine($"Findings: {resultFindings.Count}");

        foreach (var f in resultFindings)
        {
            Console.WriteLine("\tQuote: " + f.Quote);
            Console.WriteLine("\tInfo type: " + f.InfoType.Name);
            Console.WriteLine("\tLikelihood: " + f.Likelihood);
        }

        return response;
    }
}

// [END dlp_inspect_phone_number]
