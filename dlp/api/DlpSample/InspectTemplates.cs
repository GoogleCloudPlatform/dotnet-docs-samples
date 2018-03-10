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
            string projectId, 
            string displayName,
            string description,
            int likelihood, 
            int maxFindings,
            bool includeQuote,
            string infoTypes)
        {
            DlpServiceClient client = DlpServiceClient.Create();

            var request = new CreateInspectTemplateRequest
            {
                Parent = $"projects/{projectId}",
                InspectTemplate = new InspectTemplate
                {
                    DisplayName = displayName,
                    Description = description,
                    InspectConfig = new InspectConfig
                    {
                        MinLikelihood = (Likelihood)likelihood,
                        Limits = new InspectConfig.Types.FindingLimits
                        {
                            MaxFindingsPerRequest = maxFindings
                        },
                        IncludeQuote = includeQuote
                    },
                }
            };

            var response = client.CreateInspectTemplate(request);

            Console.WriteLine("Inspect template details:");
            Console.Write($"name: {response.Name}, createTime: {response.CreateTime}");

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
            Console.WriteLine($"Template {templateName} was deleted");

            return templateName;
        }
        // [END dlp_delete_inspect_template]

        // [START dlp_list_inspect_templates]
        public static object ListInspectTemplate(string projectId)
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
                    Parent = $"projects/{projectId}",
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
                        Console.WriteLine($"Info types: {string.Join(',', template.InspectConfig.InfoTypes.Select(t => t.Name))}");
                    }
                    Console.WriteLine($"Min Likelihood: {template.InspectConfig.MinLikelihood}");
                    if (template.InspectConfig.ContentOptions.Any())
                    {
                        Console.WriteLine($"Content Options: {string.Join(',', template.InspectConfig.ContentOptions.Select(o => o.ToString()))}");
                    }
                }

                nextPageToken = currentTemplatesPage.NextPageToken;
            }
            while (!string.IsNullOrEmpty(nextPageToken));

            return null;
        }
        // [END dlp_list_inspect_templates]
    }
}
