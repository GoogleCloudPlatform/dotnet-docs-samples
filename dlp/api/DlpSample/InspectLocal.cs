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

using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

namespace GoogleCloudSamples
{
    public class InspectLocal : DlpSampleBase
    {
        // [START dlp_inspect_string]
        public static object InspectString(
            string ProjectId,
            string DataValue,
            string MinLikelihood,
            int MaxFindings,
            bool IncludeQuote,
            string InfoTypesStr)
        {
            var InspectConfig = new InspectConfig
            {
                MinLikelihood = (Likelihood) Enum.Parse(typeof(Likelihood), MinLikelihood),
                Limits = new InspectConfig.Types.FindingLimits
                {
                    MaxFindingsPerRequest = MaxFindings
                },
                IncludeQuote = IncludeQuote,
                InfoTypes = { ParseInfoTypes(InfoTypesStr) }
            };
            var request = new InspectContentRequest
            {
                ParentAsProjectName = new ProjectName(ProjectId),
                Item = new ContentItem
                {
                    Value = DataValue
                },
                InspectConfig = InspectConfig
            };

            DlpServiceClient dlp = DlpServiceClient.Create();
            InspectContentResponse response = dlp.InspectContent(request);

            var findings = response.Result.Findings;
            if (findings.Count > 0)
            {
                Console.WriteLine("Findings:");
                foreach (var finding in findings)
                {
                    if (IncludeQuote)
                    {
                        Console.WriteLine($"  Quote: {finding.Quote}");
                    }
                    Console.WriteLine($"  InfoType: {finding.InfoType}");
                    Console.WriteLine($"  Likelihood: {finding.Likelihood}");
                }
            }
            else
            {
                Console.WriteLine("No findings.");
            }

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
            string ProjectId,
            string File,
            string MinLikelihood,
            int MaxFindings,
            bool IncludeQuote,
            string InfoTypesStr)
        {
            var fileStream = new FileStream(File, FileMode.Open);
            try
            {
                var inspectConfig = new InspectConfig
                {
                    MinLikelihood = (Likelihood) Enum.Parse(typeof(Likelihood), MinLikelihood),
                    Limits = new FindingLimits
                    {
                        MaxFindingsPerRequest = MaxFindings
                    },
                    IncludeQuote = IncludeQuote,
                    InfoTypes = { ParseInfoTypes(InfoTypesStr) }
                };
                DlpServiceClient dlp = DlpServiceClient.Create();
                InspectContentResponse response = dlp.InspectContent(new InspectContentRequest
                {
                    ParentAsProjectName = new ProjectName(ProjectId),
                    Item = new ContentItem
                    {
                        ByteItem = new ByteContentItem
                        {
                            Data = ByteString.FromStream(fileStream),
                            Type = s_fileTypes.GetValueOrDefault(
                                    new FileInfo(File).Extension.ToLower(),
                                    ByteContentItem.Types.BytesType.Unspecified
                            )
                        }
                    },
                    InspectConfig = inspectConfig
                });

                var findings = response.Result.Findings;
                if (findings.Count > 0) {
                    Console.WriteLine("Findings:");
                    foreach (var finding in findings)
                    {
                        if (IncludeQuote) {
                            Console.WriteLine($"  Quote: {finding.Quote}");
                        }
                        Console.WriteLine($"  InfoType: {finding.InfoType}");
                        Console.WriteLine($"  Likelihood: {finding.Likelihood}");
                    }
                } else {
                    Console.WriteLine("No findings.");
                }

                return 0;
            }
            finally
            {
                fileStream.Close();
            }
        }
        // [END dlp_inspect_file]
    }
}
