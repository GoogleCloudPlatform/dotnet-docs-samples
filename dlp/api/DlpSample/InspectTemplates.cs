// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax;
using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleCloudSamples
{
    class InspectTemplates
    {
        // [START dlp_create_inspect_template]
        public static string CreateInspectTemplate(
            string ProjectId,
            string TemplateId,
            string DisplayName,
            string Description,
            string Likelihood,
            int MaxFindings,
            bool IncludeQuote)
        {
            DlpServiceClient client = DlpServiceClient.Create();

            var request = new CreateInspectTemplateRequest
            {
                Parent = $"projects/{ProjectId}",
                InspectTemplate = new InspectTemplate
                {
                    DisplayName = DisplayName,
                    Description = Description,
                    InspectConfig = new InspectConfig
                    {
                        MinLikelihood = (Likelihood) Enum.Parse(typeof(Likelihood), Likelihood),
                        Limits = new InspectConfig.Types.FindingLimits
                        {
                            MaxFindingsPerRequest = MaxFindings
                        },
                        IncludeQuote = IncludeQuote
                    },
                },
                TemplateId = TemplateId
            };

            var response = client.CreateInspectTemplate(request);

            Console.WriteLine($"Successfully created template {response.Name}.");

            return response.Name;
        }
        // [END dlp_create_inspect_template]

        // [START dlp_delete_inspect_template]
        public static object DeleteInspectTemplate(string projectId, string templateName)
        {
            DlpServiceClient client = DlpServiceClient.Create();

            var request = new DeleteInspectTemplateRequest
            {
                Name = templateName
            };

            client.DeleteInspectTemplate(request);
            Console.WriteLine($"Successfully deleted template {templateName}.");

            return templateName;
        }
        // [END dlp_delete_inspect_template]

        // [START dlp_list_inspect_templates]
        public static object ListInspectTemplate(string ProjectId)
        {
            DlpServiceClient client = DlpServiceClient.Create();

            var response = client.ListInspectTemplates(
                new ListInspectTemplatesRequest
                {
                    Parent = $"projects/{ProjectId}",
                }
            );
            foreach (var template in response)
            {
                Console.WriteLine($"Template {template.Name}:");
                Console.WriteLine($"\tDisplay Name: {template.DisplayName}");
                Console.WriteLine($"\tDescription: {template.Description}");
                Console.WriteLine($"\tCreated: {template.CreateTime}");
                Console.WriteLine($"\tUpdated: {template.UpdateTime}");
                Console.WriteLine("Configuration:");
                Console.WriteLine($"\tMin Likelihood: {template.InspectConfig.MinLikelihood}");
                Console.WriteLine($"\tInclude quotes: {template.InspectConfig.IncludeQuote}");
                Console.WriteLine($"\tMax findings per request: {template.InspectConfig.Limits.MaxFindingsPerRequest}");
            }

            return null;
        }
        // [END dlp_list_inspect_templates]

        // An example of ListInspectTemplates, but using paging
        // Not intended for inclusion in the documentation
        public static object ListInspectTemplatesPaging(string ProjectId)
        {
            DlpServiceClient client = DlpServiceClient.Create();

            // Read the templates in batches of 10 using paging.
            const int batchSize = 10;
            string nextPageToken = string.Empty;
            Page<InspectTemplate> currentTemplatesPage = null;
            do
            {
                var request = new ListInspectTemplatesRequest
                {
                    Parent = $"projects/{ProjectId}",
                    PageToken = nextPageToken,
                };

                // This is the actual API call to DLP
                var response = client.ListInspectTemplates(request);

                currentTemplatesPage = response.ReadPage(batchSize); ;
                foreach (var template in currentTemplatesPage)
                {
                    Console.WriteLine("Inspect Template Info:");
                    Console.WriteLine($"\tname: {template.Name}");
                    Console.WriteLine($"\tdisplayName: {template.DisplayName}");
                    Console.WriteLine($"\tdescription: {template.Description}");
                    Console.WriteLine($"\tcreateTime: {template.CreateTime}");
                    Console.WriteLine("Configuration:");
                    if (template.InspectConfig.InfoTypes.Any())
                    {
                        Console.WriteLine(
                            $"\tInfo types: {string.Join(',', template.InspectConfig.InfoTypes.Select(t => t.Name))}");
                    }
                    Console.WriteLine($"Min Likelihood: {template.InspectConfig.MinLikelihood}");
                    if (template.InspectConfig.ContentOptions.Any())
                    {
                        Console.WriteLine($"\tContent Options: {string.Join(',', template.InspectConfig.ContentOptions.Select(o => o.ToString()))}");
                    }
                    Console.WriteLine();
                }

                nextPageToken = currentTemplatesPage.NextPageToken;
            }
            while (!string.IsNullOrEmpty(nextPageToken));

            return null;
        }
    }
}
