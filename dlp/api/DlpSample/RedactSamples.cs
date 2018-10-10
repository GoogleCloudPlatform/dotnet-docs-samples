using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GoogleCloudSamples
{
    class RedactSamples
    {
        // [START redact_image]
        public static object RedactFromImage(string projectId, string originalImagePath, string redactedImagePath)
        {
            var request = new RedactImageRequest
            {
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(projectId),
                InspectConfig = new InspectConfig
                {
                    MinLikelihood = Likelihood.Likely,
                    Limits = new InspectConfig.Types.FindingLimits() { MaxFindingsPerItem = 5 },
                    IncludeQuote = true,
                    InfoTypes =
                    {
                        new InfoType { Name = "PHONE_NUMBER" },
                        new InfoType { Name = "EMAIL_ADDRESS" }
                    }
                },
                ByteItem = new ByteContentItem
                {
                    Type = ByteContentItem.Types.BytesType.ImagePng,
                    Data = ByteString.FromStream(new FileStream(originalImagePath, FileMode.Open))
                },
            };

            DlpServiceClient client = DlpServiceClient.Create();
            var response = client.RedactImage(request);

            Console.WriteLine($"Extracted text: {response.ExtractedText}");

            // Writes redacted image into file
            response.RedactedImage.WriteTo(new FileStream(redactedImagePath, FileMode.Create, FileAccess.Write));

            return 0;
        }
    }
}
