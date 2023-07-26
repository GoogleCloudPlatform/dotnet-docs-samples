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

// [START dlp_inspect_column_values_w_custom_hotwords]


using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;
using static Google.Cloud.Dlp.V2.CustomInfoType.Types;

public class InspectTableWithCustomHotwords
{
    public static InspectResult InspectTable(
        string projectId,
        Table tableToInspect = null,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value{ StringValue = "111-11-1111" },
                new Value { StringValue = "222-22-2222" }
            };
            tableToInspect = new Table
            {
                Headers =
                {
                    new FieldId { Name = "Fake Social Security Number" },
                    new FieldId { Name = "Real Social Security Number" }
                },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } }
                }
            };
        }

        // Specify the table and construct the content item.
        var contentItem = new ContentItem { Table = tableToInspect };

        // Specify the type of info to be inspected.
        var infotypes = infoTypes ?? new InfoType[] { new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" } };

        // Construct the Inspection Rule Set by specifying the hotword rule as detection rule.
        var ruleSet = new InspectionRuleSet[]
        {
            new InspectionRuleSet
            {
                InfoTypes = { infotypes },
                Rules =
                {
                    new InspectionRule
                    {
                        HotwordRule = new DetectionRule.Types.HotwordRule
                        {
                            HotwordRegex = new Regex
                            {
                                Pattern = "(Fake Social Security Number)"
                            },
                            LikelihoodAdjustment = new DetectionRule.Types.LikelihoodAdjustment
                            {
                                FixedLikelihood = Likelihood.VeryUnlikely
                            },
                            Proximity = new DetectionRule.Types.Proximity
                            {
                                WindowBefore = 1
                            }
                        }
                    }
                }
            }
        };

        // Construct the request.
        var request = new InspectContentRequest
        {
            InspectConfig = new InspectConfig
            {
                InfoTypes = { infotypes },
                IncludeQuote = true,
                MinLikelihood = Likelihood.Possible,
                RuleSet = { ruleSet }
            },
            ParentAsLocationName = new LocationName(projectId, "global"),
            Item = contentItem
        };

        // Call the API.
        InspectContentResponse response = dlp.InspectContent(request);

        // Inspect the results.
        var resultFindings = response.Result.Findings;

        Console.WriteLine($"Findings: {resultFindings.Count}");

        foreach (var f in resultFindings)
        {
            Console.WriteLine("\tQuote: " + f.Quote);
            Console.WriteLine("\tInfo type: " + f.InfoType.Name);
            Console.WriteLine("\tLikelihood: " + f.Likelihood);
        }

        return response.Result;
    }
}

// [END dlp_inspect_column_values_w_custom_hotwords]
