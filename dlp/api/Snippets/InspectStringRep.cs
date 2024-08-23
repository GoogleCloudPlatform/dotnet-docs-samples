// Copyright (c) 2024 Google LLC.
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

// [START dlp_inspect_string_rep]

using System;
using System.Collections.Generic;
using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

public class InspectStringRep
{
    public static InspectContentResponse Inspect(
        string projectId,
        string repLocation,
        string dataValue,
        string minLikelihood,
        int maxFindings,
        bool includeQuote,
        IEnumerable<InfoType> infoTypes,
        IEnumerable<CustomInfoType> customInfoTypes)
    {
        var inspectConfig = new InspectConfig
        {
            MinLikelihood = (Likelihood)Enum.Parse(typeof(Likelihood), minLikelihood, true),
            Limits = new FindingLimits
            {
                MaxFindingsPerRequest = maxFindings
            },
            IncludeQuote = includeQuote,
            InfoTypes = { infoTypes },
            CustomInfoTypes = { customInfoTypes }
        };
        var request = new InspectContentRequest
        {
            Parent = new LocationName(projectId, repLocation).ToString(),
            Item = new ContentItem
            {
                Value = dataValue
            },
            InspectConfig = inspectConfig
        };

        var dlp = new DlpServiceClientBuilder
        {
            Endpoint = $"dlp.{repLocation}.rep.googleapis.com"
        }.Build();

        var response = dlp.InspectContent(request);

        PrintResponse(includeQuote, response);

        return response;
    }

    private static void PrintResponse(bool includeQuote, InspectContentResponse response)
    {
        var findings = response.Result.Findings;
        if (findings.Any())
        {
            Console.WriteLine("Findings:");
            foreach (var finding in findings)
            {
                if (includeQuote)
                {
                    Console.WriteLine($"  Quote: {finding.Quote}");
                }
                Console.WriteLine($"  InfoType: {finding.InfoType}");
                Console.WriteLine($"  Likelihood: {finding.Likelihood}");
            }
        }
        else
        {
            Console.WriteLine("No findings.");
        }
    }
}
// [END dlp_inspect_string_rep]
