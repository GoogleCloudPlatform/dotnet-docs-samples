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

// [START dlp_inspect_with_stored_infotype]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class InspectDataWithStoredInfotypes
{
    public static InspectContentResponse Inspect(
        string projectId,
        string storedInfotypePath,
        string text,
        InfoType infoType = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the infotype if null.
        var infotype = infoType ?? new InfoType { Name = "GITHUB_LOGINS" };

        // Construct the inspect config using stored infotype.
        var inspectConfig = new InspectConfig
        {
            CustomInfoTypes =
            {
                new CustomInfoType
                {
                    InfoType = infotype,
                    StoredType = new StoredType { Name = storedInfotypePath }
                }
            },
            IncludeQuote = true
        };

        // Construct the request using inspect config.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectConfig = inspectConfig,
            Item = new ContentItem { Value = text }
        };

        // Call the API.
        InspectContentResponse response = dlp.InspectContent(request);

        // Inspect the results.
        var findings = response.Result.Findings;
        Console.WriteLine($"Findings: {findings.Count}");
        foreach (var f in findings)
        {
            Console.WriteLine("\tQuote: " + f.Quote);
            Console.WriteLine("\tInfo type: " + f.InfoType.Name);
            Console.WriteLine("\tLikelihood: " + f.Likelihood);
        }

        return response;
    }
}

// [END dlp_inspect_with_stored_infotype]

