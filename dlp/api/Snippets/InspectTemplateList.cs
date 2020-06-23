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

// [START dlp_list_inspect_templates]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;
using Google.Api.Gax;

public class InspectTemplateList
{
    public static PagedEnumerable<ListInspectTemplatesResponse, InspectTemplate> List(string projectId)
    {
        var client = DlpServiceClient.Create();

        var response = client.ListInspectTemplates(
            new ListInspectTemplatesRequest
            {
                Parent = new LocationName(projectId, "global").ToString(),
            }
        );

        // Uncomment to list templates
        //PrintTemplates(response);

        return response;
    }

    public static void PrintTemplates(PagedEnumerable<ListInspectTemplatesResponse, InspectTemplate> response)
    {
        foreach (var template in response)
        {
            Console.WriteLine($"Template {template.Name}:");
            Console.WriteLine($"\tDisplay Name: {template.DisplayName}");
            Console.WriteLine($"\tDescription: {template.Description}");
            Console.WriteLine($"\tCreated: {template.CreateTime}");
            Console.WriteLine($"\tUpdated: {template.UpdateTime}");
            Console.WriteLine("Configuration:");
            Console.WriteLine($"\tMin Likelihood: {template.InspectConfig?.MinLikelihood}");
            Console.WriteLine($"\tInclude quotes: {template.InspectConfig?.IncludeQuote}");
            Console.WriteLine($"\tMax findings per request: {template.InspectConfig?.Limits.MaxFindingsPerRequest}");
        }
    }
}

// [END dlp_list_inspect_templates]
