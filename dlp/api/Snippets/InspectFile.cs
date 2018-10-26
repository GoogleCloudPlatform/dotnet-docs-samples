// Copyright (c) 2018 Google LLC.
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

// [START dlp_inspect_file]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using static Google.Cloud.Dlp.V2.ByteContentItem.Types;

class DlpInspectFile
{
    /// <summary>
    /// Inspects the provided file for sensitive data.
    ///</summary>
    /// <param name="projectId">Your Google Cloud Project ID.</param>
    /// <param name="filePath">The path to the specified file to inspect.</param>
    public IEnumerable<Finding> InspectFile(
        string projectId = "YOUR-PROJECT-ID",
        string filePath = "path/to/image.png")
    {
        // Instantiate a client.
        DlpServiceClient dlp = DlpServiceClient.Create();

        // Get the bytes from the file.
        ByteString fileBytes;
        using (Stream f = new FileStream(filePath, FileMode.Open))
        {
            fileBytes = ByteString.FromStream(f);
        }

        // Construct a request.
        var request = new InspectContentRequest
        {
            ParentAsProjectName = new ProjectName(projectId),
            Item = new ContentItem
            {
                ByteItem = new ByteContentItem()
                {
                    Data = fileBytes,
                    Type = BytesType.ImagePng
                }
            },
            InspectConfig = new InspectConfig
            {
                // The info types of information to match
                InfoTypes = {
                    new InfoType { Name = "PHONE_NUMBER" },
                    new InfoType { Name = "EMAIL_ADDRESS" },
                    new InfoType { Name = "CREDIT_CARD_NUMBER" }
                },
                // The minimum likelihood before returning a match
                MinLikelihood = Likelihood.Unspecified,
                // Whether to include the matching string
                IncludeQuote = true,
                Limits = new InspectConfig.Types.FindingLimits
                {
                    // The maximum number of findings to report per request
                    // (0 = server maximum)
                    MaxFindingsPerRequest = 0
                }
            }
        };

        // Execute request
        InspectContentResponse response = dlp.InspectContent(request);

        // Inspect response
        var findings = response.Result.Findings;
        if (findings.Count > 0)
        {
            Console.WriteLine("Findings:");
            foreach (var finding in findings)
            {
                Console.WriteLine($"Quote: {finding.Quote}");
                Console.WriteLine($"InfoType: {finding.InfoType}");
                Console.WriteLine($"Likelihood: {finding.Likelihood}");
            }
        }
        else
        {
            Console.WriteLine("No findings.");
        }
        return findings;
    }
}

// [END dlp_inspect_file]