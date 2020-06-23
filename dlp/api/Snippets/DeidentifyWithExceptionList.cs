using System.Text.RegularExpressions;
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

// [START dlp_deidentify_exception_list]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyWithExceptionList
{
    public static DeidentifyContentResponse Deidentify(string projectId, string text)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        var contentItem = new ContentItem { Value = text };

        var wordList = new CustomInfoType.Types.Dictionary.Types.WordList
        {
            Words = { new string[] { "jack@example.org", "jill@example.org" } }
        };

        var exclusionRule = new ExclusionRule
        {
            MatchingType = MatchingType.FullMatch,
            Dictionary = new CustomInfoType.Types.Dictionary
            {
                WordList = wordList
            }
        };

        var infoType = new InfoType { Name = "EMAIL_ADDRESS" };

        var inspectionRuleSet = new InspectionRuleSet
        {
            InfoTypes = { infoType },
            Rules = { new InspectionRule { ExclusionRule = exclusionRule } }
        };

        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoType },
            RuleSet = { inspectionRuleSet }
        };
        var primitiveTransformation = new PrimitiveTransformation
        {
            ReplaceWithInfoTypeConfig = new ReplaceWithInfoTypeConfig { }
        };

        var transformation = new InfoTypeTransformations.Types.InfoTypeTransformation
        {
            InfoTypes = { infoType },
            PrimitiveTransformation = primitiveTransformation
        };

        var deidentifyConfig = new DeidentifyConfig
        {
            InfoTypeTransformations = new InfoTypeTransformations
            {
                Transformations = { transformation }
            }
        };

        var request = new DeidentifyContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            InspectConfig = inspectConfig,
            DeidentifyConfig = deidentifyConfig,
            Item = contentItem
        };

        // Call the API.
        var response = dlp.DeidentifyContent(request);

        // Inspect the results.
        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_deidentify_exception_list]
