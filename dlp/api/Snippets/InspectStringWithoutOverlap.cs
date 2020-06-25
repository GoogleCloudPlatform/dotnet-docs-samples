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

// [START dlp_inspect_string_without_overlap]

using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using static Google.Cloud.Dlp.V2.CustomInfoType.Types;

public class InspectStringWithoutOverlap
{
    public static InspectContentResponse Inspect(string projectId, string textToInspect)
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
        var dlp = DlpServiceClient.Create();

        // Specify the type and content to be inspected.
        var byteContentItem = new ByteContentItem
        {
            Type = ByteContentItem.Types.BytesType.TextUtf8,
            Data = Google.Protobuf.ByteString.CopyFromUtf8(textToInspect)
        };
        var contentItem = new ContentItem
        {
            ByteItem = byteContentItem
        };

        // Specify the type of info the inspection will look for.
        // See https://cloud.google.com/dlp/docs/infotypes-reference for complete list of info types.
        var infoTypes = new string[] { "DOMAIN_NAME", "EMAIL_ADDRESS" }.Select(it => new InfoType { Name = it });

        // Define a custom info type to exclude email addresses
        var customInfoType = new CustomInfoType
        {
            InfoType = new InfoType { Name = "EMAIL_ADDRESS" },
            ExclusionType = ExclusionType.Exclude
        };

        // Exclude EMAIL_ADDRESS matches
        var exclusionRule = new ExclusionRule
        {
            ExcludeInfoTypes = new ExcludeInfoTypes
            {
                InfoTypes = { new InfoType { Name = "EMAIL_ADDRESS" } }
            },
            MatchingType = MatchingType.PartialMatch
        };

        // Construct a ruleset that applies the exclusion rule to the DOMAIN_NAME infotype.
        // If a DOMAIN_NAME match is part of an EMAIL_ADDRESS match, the DOMAIN_NAME match will
        // be excluded.
        var ruleSet = new InspectionRuleSet
        {
            InfoTypes = { new InfoType { Name = "DOMAIN_NAME" } },
            Rules = { new InspectionRule { ExclusionRule = exclusionRule } }
        };

        // Construct the configuration for the Inspect request, including the ruleset.
        var config = new InspectConfig
        {
            InfoTypes = { infoTypes },
            CustomInfoTypes = { customInfoType },
            IncludeQuote = true,
            RuleSet = { ruleSet }
        };

        // Construct the Inspect request to be sent by the client.
        var request = new InspectContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            Item = contentItem,
            InspectConfig = config
        };

        // Use the client to send the API request.
        var response = dlp.InspectContent(request);

        return response;
    }
}
// [END dlp_inspect_string_without_overlap]
