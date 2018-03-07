using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GoogleCloudSamples
{
    public class InspectLocal
    {
        static IEnumerable<InfoType> ParseInfoTypes(string infoTypesStr)
        {
            return infoTypesStr.Split(',').Select(str =>
            {
                try
                {
                    return InfoType.Parser.ParseJson($"{{\"name\": \"{str}\"}}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse infoType {str}: {e}");
                    return null;
                }
            }).Where(it => it != null);
        }

        public static object InspectString(string projectId, string value, int minLikelihood, int maxFindings,
            bool includeQuote, string infoTypes)
        {
            return Inspect(new InspectContentRequest
            {
                Parent = $"projects/{projectId}",
                Item = new ContentItem
                {
                    Value = value
                }
            }, minLikelihood, maxFindings, includeQuote, infoTypes);
        }

        static readonly Dictionary<string, ByteContentItem.Types.BytesType> fileTypes =
            new Dictionary<string, ByteContentItem.Types.BytesType>()
        {
            {"bmp", ByteContentItem.Types.BytesType.ImageBmp},
            {"jpg", ByteContentItem.Types.BytesType.ImageJpeg},
            {"jpeg", ByteContentItem.Types.BytesType.ImageJpeg},
            {"png", ByteContentItem.Types.BytesType.ImagePng},
            {"svg", ByteContentItem.Types.BytesType.ImageSvg},
            {"txt", ByteContentItem.Types.BytesType.TextUtf8}
        };

        public static object InspectFile(string projectId, string file, int minLikelihood, int maxFindings, 
            bool includeQuote, string infoTypes)
        {
            var fileStream = new FileStream(file, FileMode.Open);
            try
            {
                return Inspect(new InspectContentRequest
                {
                    Parent = $"projects/{projectId}",
                    Item = new ContentItem
                    {
                        ByteItem = new ByteContentItem
                        {
                            Data = ByteString.FromStream(fileStream),
                            Type = fileTypes.GetValueOrDefault(new FileInfo(file).Extension.ToLower(),
                                    ByteContentItem.Types.BytesType.Unspecified)
                        }
                    }
                }, minLikelihood, maxFindings, includeQuote, infoTypes);
            }
            finally
            {
                fileStream.Close();
            }
        }

        private static object Inspect(InspectContentRequest request, int minLikelihood, int maxFindings, 
            bool includeQuote, string infoTypes)
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
            inspectConfig.InfoTypes.AddRange(ParseInfoTypes(infoTypes));
            request.InspectConfig = inspectConfig;
            DlpServiceClient dlp = DlpServiceClient.Create();
            InspectContentResponse response = dlp.InspectContent(request);
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
