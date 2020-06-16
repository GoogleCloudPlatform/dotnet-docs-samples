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

// [START dlp_inspect_string_multiple_rules]

using System;
using System.Text.RegularExpressions;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using static Google.Cloud.Dlp.V2.CustomInfoType.Types;

public class InspectStringMultipleRules
{
    public static InspectContentResponse Inspect(string projectId, string textToInspect)
    {
        var dlp = DlpServiceClient.Create();

        var byteContentItem = new ByteContentItem
        {
            Type = ByteContentItem.Types.BytesType.TextUtf8,
            Data = Google.Protobuf.ByteString.CopyFromUtf8(textToInspect)
        };

        var contentItem = new ContentItem
        {
            ByteItem = byteContentItem
        };

        var patientRule = new DetectionRule.Types.HotwordRule
        {
            HotwordRegex = new CustomInfoType.Types.Regex { Pattern = "patient" },
            Proximity = new DetectionRule.Types.Proximity { WindowBefore = 10 },
            LikelihoodAdjustment = new DetectionRule.Types.LikelihoodAdjustment { FixedLikelihood = Likelihood.VeryLikely }
        };

        var doctorRule = new DetectionRule.Types.HotwordRule
        {
            HotwordRegex = new CustomInfoType.Types.Regex { Pattern = "doctor" },
            Proximity = new DetectionRule.Types.Proximity { WindowBefore = 10 },
            LikelihoodAdjustment = new DetectionRule.Types.LikelihoodAdjustment { FixedLikelihood = Likelihood.Unlikely }
        };

        // Construct exclusion rules
        var quasimodoRule = new ExclusionRule
        {
            Dictionary = new Dictionary { WordList = new Dictionary.Types.WordList { Words = { "Quasimodo" } } },
            MatchingType = MatchingType.PartialMatch
        };

        var redactedRule = new ExclusionRule
        {
            Regex = new CustomInfoType.Types.Regex { Pattern = "REDACTED" },
            MatchingType = MatchingType.PartialMatch
        };

        var infoType = new InfoType { Name = "PERSON_NAME" };

        var inspectionRuleSet = new InspectionRuleSet
        {
            InfoTypes = { infoType },
            Rules =
            {
                new InspectionRule { HotwordRule = patientRule },
                new InspectionRule { HotwordRule = doctorRule},
                new InspectionRule { ExclusionRule = quasimodoRule },
                new InspectionRule { ExclusionRule = redactedRule }
            }
        };

        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoType },
            IncludeQuote = true,
            RuleSet = { inspectionRuleSet }
        };

        var request = new InspectContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            Item = contentItem,
            InspectConfig = inspectConfig
        };

        var response = dlp.InspectContent(request);

        Console.WriteLine($"Findings: {response.Result.Findings.Count}");
        foreach (var f in response.Result.Findings)
        {
            Console.WriteLine("\tQuote: " + f.Quote);
            Console.WriteLine("\tInfo type: " + f.InfoType.Name);
            Console.WriteLine("\tLikelihood: " + f.Likelihood);
        }

        return response;
    }
}
// [END dlp_inspect_string_multiple_rules]
