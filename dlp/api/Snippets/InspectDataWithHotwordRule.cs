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

// [START dlp_inspect_hotword_rule]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using static Google.Cloud.Dlp.V2.CustomInfoType.Types;

public class InspectDataWithHotwordRule
{
    public static InspectContentResponse InspectDataHotwordRule(
        string projectId,
        string text,
        string customRegex,
        string hotwordRegex,
        InfoType infoType = null)
    {
        // Instantiate dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the content item.
        var contentItem = new ContentItem
        {
            ByteItem = new ByteContentItem
            {
                Type = ByteContentItem.Types.BytesType.TextUtf8,
                Data = Google.Protobuf.ByteString.CopyFromUtf8(text)
            }
        };

        // Construct the info type if null.
        var infotype = infoType ?? new InfoType { Name = "C_MRN" };

        // Construct the custom regex detector.
        var customInfoType = new CustomInfoType
        {
            InfoType = infotype,
            Regex = new Regex { Pattern = customRegex },
            Likelihood = Likelihood.Possible
        };

        // Construct hotword rule.
        var hotwordRule = new DetectionRule.Types.HotwordRule
        {
            HotwordRegex = new Regex { Pattern = hotwordRegex },
            LikelihoodAdjustment = new DetectionRule.Types.LikelihoodAdjustment
            {
                FixedLikelihood = Likelihood.VeryLikely
            },
            Proximity = new DetectionRule.Types.Proximity
            {
                WindowBefore = 10
            }
        };

        // Construct the rule set for the inspect config.
        var inspectionRuleSet = new InspectionRuleSet
        {
            InfoTypes = { infotype },
            Rules =
            {
                new InspectionRule
                {
                    HotwordRule = hotwordRule
                }
            }
        };

        // Construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            CustomInfoTypes = { customInfoType },
            IncludeQuote = true,
            RuleSet = { inspectionRuleSet },
        };

        // Construct the request.
        var request = new InspectContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            Item = contentItem,
            InspectConfig = inspectConfig
        };

        // Call the API.
        var response = dlp.InspectContent(request);

        // Inspect the response.
        Console.WriteLine($"Findings: {response.Result.Findings.Count}");
        foreach (var f in response.Result.Findings)
        {
            Console.WriteLine("Quote: " + f.Quote);
            Console.WriteLine("Info type: " + f.InfoType.Name);
            Console.WriteLine("Likelihood: " + f.Likelihood);
        }
        return response;
    }
}

// [END dlp_inspect_hotword_rule]
