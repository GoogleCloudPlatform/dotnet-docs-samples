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

// [START dlp_create_inspect_template]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;

public class InspectTemplateCreate
{
    public static InspectTemplate Create(
        string projectId,
        string templateId,
        string displayName,
        string description,
        Likelihood likelihood,
        int maxFindings,
        bool includeQuote)
    {
        var client = DlpServiceClient.Create();

        var request = new CreateInspectTemplateRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            InspectTemplate = new InspectTemplate
            {
                DisplayName = displayName,
                Description = description,
                InspectConfig = new InspectConfig
                {
                    MinLikelihood = likelihood,
                    Limits = new InspectConfig.Types.FindingLimits
                    {
                        MaxFindingsPerRequest = maxFindings
                    },
                    IncludeQuote = includeQuote
                },
            },
            TemplateId = templateId
        };

        var response = client.CreateInspectTemplate(request);

        Console.WriteLine($"Successfully created template {response.Name}.");

        return response;
    }
}

// [END dlp_create_inspect_template]
