using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GoogleCloudSamples
{
    public class InspectLocal : DlpSampleBase
    {
        // [START dlp_inspect_string]
        public static object InspectString(
            string projectId,
            string value,
            int minLikelihood,
            int maxFindings,
            bool includeQuote,
            string infoTypesStr)
        {
            var inspectConfig = new InspectConfig
            {
                MinLikelihood = (Likelihood)minLikelihood,
                Limits = new InspectConfig.Types.FindingLimits
                {
                    MaxFindingsPerRequest = maxFindings
                },
                IncludeQuote = includeQuote
            };
            inspectConfig.InfoTypes.AddRange(ParseInfoTypes(infoTypesStr));
            var request = new InspectContentRequest
            {
                Parent = $"projects/{projectId}",
                Item = new ContentItem
                {
                    Value = value
                },
                InspectConfig = inspectConfig
            };

            DlpServiceClient dlp = DlpServiceClient.Create();
            InspectContentResponse response = dlp.InspectContent(request);
            PrintOutput(response);
            return 0;
        }
        // [END dlp_inspect_string]

        // [START dlp_inspect_file]
        static readonly Dictionary<string, ByteContentItem.Types.BytesType> s_fileTypes =
            new Dictionary<string, ByteContentItem.Types.BytesType>()
        {
            {"bmp", ByteContentItem.Types.BytesType.ImageBmp},
            {"jpg", ByteContentItem.Types.BytesType.ImageJpeg},
            {"jpeg", ByteContentItem.Types.BytesType.ImageJpeg},
            {"png", ByteContentItem.Types.BytesType.ImagePng},
            {"svg", ByteContentItem.Types.BytesType.ImageSvg},
            {"txt", ByteContentItem.Types.BytesType.TextUtf8}
        };

        public static object InspectFile(
            string projectId,
            string file,
            int minLikelihood,
            int maxFindings,
            bool includeQuote,
            string infoTypesStr)
        {
            var fileStream = new FileStream(file, FileMode.Open);
            try
            {
                var inspectConfig = new InspectConfig
                {
                    MinLikelihood = (Likelihood)minLikelihood,
                    Limits = new InspectConfig.Types.FindingLimits
                    {
                        MaxFindingsPerRequest = maxFindings
                    },
                    IncludeQuote = includeQuote
                };
                inspectConfig.InfoTypes.AddRange(ParseInfoTypes(infoTypesStr));
                DlpServiceClient dlp = DlpServiceClient.Create();
                InspectContentResponse response = dlp.InspectContent(new InspectContentRequest
                {
                    Parent = $"projects/{projectId}",
                    Item = new ContentItem
                    {
                        ByteItem = new ByteContentItem
                        {
                            Data = ByteString.FromStream(fileStream),
                            Type = s_fileTypes.GetValueOrDefault(new FileInfo(file).Extension.ToLower(),
                                    ByteContentItem.Types.BytesType.Unspecified)
                        }
                    },
                    InspectConfig = inspectConfig
                });
                PrintOutput(response);
                return 0;
            }
            finally
            {
                fileStream.Close();
            }
        }
        // [END dlp_inspect_file]

        private static object PrintOutput(InspectContentResponse response)
        {
            var count = 0;
            var findingsStr = new StringBuilder();
            foreach (var finding in response.Result.Findings)
            {
                findingsStr.Append($"\nFinding {count++}: \n\t{finding}");
            }
            var wereOrNotTruncated = "were" + (response.Result.FindingsTruncated ? "" : " not") + " truncated";
            Console.WriteLine($"Found {count} results, and results {wereOrNotTruncated}: {findingsStr.ToString()}");
            return 0;
        }
    }
}
