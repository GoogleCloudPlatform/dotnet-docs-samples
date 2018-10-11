// [START dlp_inspect_file]
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using static Google.Cloud.Dlp.V2.ByteContentItem.Types;

public class DlpInspectFile
{
    /// <summary>
    /// Inspects the provided file for sensitive data.
    ///</summary>
    /// <param name="projectId">Your Google Cloud Project ID.</param>
    /// <param name="filePath">The path to the specified file to inspect.</param>
    /// <param name="fileType">The type of the specifed file.</param>
    public IEnumerable<Finding> InspectFile(
        string projectId = "YOUR-PROJECT-ID",
        string filePath = "path/to/image.png",
        BytesType fileType = BytesType.ImagePng
    )
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
                    Type = fileType
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

        // Return findings
        return findings;
    }
}
// [END dlp_inspect_file]