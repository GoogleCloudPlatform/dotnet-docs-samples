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
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;
using System.Linq;

namespace GoogleCloudSamples
{
    class DeIdentify
    {
        // [START dlp_deidentify_masking]
        public static object DeidMask(
            string projectId,
            string dataValue,
            IEnumerable<InfoType> infoTypes,
            string maskingCharacter,
            int numberToMask,
            bool reverseOrder)
        {
            var request = new DeidentifyContentRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                InspectConfig = new InspectConfig
                {
                    InfoTypes = { infoTypes }
                },
                DeidentifyConfig = new DeidentifyConfig
                {
                    InfoTypeTransformations = new InfoTypeTransformations
                    {
                        Transformations = {
                            new InfoTypeTransformations.Types.InfoTypeTransformation
                            {
                                PrimitiveTransformation = new PrimitiveTransformation
                                {
                                    CharacterMaskConfig = new CharacterMaskConfig
                                    {
                                        MaskingCharacter = maskingCharacter,
                                        NumberToMask = numberToMask,
                                        ReverseOrder = reverseOrder
                                    }
                                }
                            }
                        }
                    }
                },
                Item = new ContentItem
                {
                    Value = dataValue
                }
            };

            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.DeidentifyContent(request);

            Console.WriteLine($"Deidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_deidentify_masking]

        // [START dlp_deidentify_fpe]
        public static object DeidFpe(
            string projectId,
            string dataValue,
            IEnumerable<InfoType> infoTypes,
            string keyName,
            string wrappedKey,
            string alphabet)
        {
            var deidentifyConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations
                {
                    Transformations =
                    {
                        new InfoTypeTransformations.Types.InfoTypeTransformation
                        {
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                CryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
                                {
                                    CommonAlphabet = (FfxCommonNativeAlphabet) Enum.Parse(typeof(FfxCommonNativeAlphabet), alphabet),
                                    CryptoKey = new CryptoKey
                                    {
                                        KmsWrapped = new KmsWrappedCryptoKey
                                        {
                                            CryptoKeyName = keyName,
                                            WrappedKey = ByteString.FromBase64(wrappedKey)
                                        }
                                    },
                                    SurrogateInfoType = new InfoType
                                    {
                                        Name = "TOKEN"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.DeidentifyContent(
                new DeidentifyContentRequest
                {
                    ParentAsProjectName = new ProjectName(projectId),
                    InspectConfig = new InspectConfig
                    {
                        InfoTypes = { infoTypes }
                    },
                    DeidentifyConfig = deidentifyConfig,
                    Item = new ContentItem { Value = dataValue }
                });

            Console.WriteLine($"Deidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_deidentify_fpe]

        // [START dlp_reidentify_fpe]
        public static object ReidFpe(
            string projectId,
            string dataValue,
            string keyName,
            string wrappedKey,
            string alphabet)
        {
            var reidentifyConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations
                {
                    Transformations =
                    {
                        new InfoTypeTransformations.Types.InfoTypeTransformation
                        {
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                CryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
                                {
                                    CommonAlphabet = (FfxCommonNativeAlphabet) Enum.Parse(typeof(FfxCommonNativeAlphabet), alphabet),
                                    CryptoKey = new CryptoKey
                                    {
                                        KmsWrapped = new KmsWrappedCryptoKey
                                        {
                                            CryptoKeyName = keyName,
                                            WrappedKey = ByteString.FromBase64(wrappedKey)
                                        }
                                    },
                                    SurrogateInfoType = new InfoType
                                    {
                                        Name = "TOKEN"
                                    }
                                }
                            },
                            InfoTypes =
                            {
                                new InfoType { Name = "TOKEN" }
                            }
                        }
                    }
                }
            };

            var inspectConfig = new InspectConfig
            {
                CustomInfoTypes =
                {
                    new CustomInfoType
                    {
                        InfoType = new InfoType
                        {
                            Name = "TOKEN"
                        },
                        SurrogateType = new CustomInfoType.Types.SurrogateType()
                    }
                }
            };

            var dlp = DlpServiceClient.Create();
            var response = dlp.ReidentifyContent(new ReidentifyContentRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                InspectConfig = inspectConfig,
                ReidentifyConfig = reidentifyConfig,
                Item = new ContentItem { Value = dataValue }
            });

            Console.WriteLine($"Reidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_reidentify_fpe]

        // [START dlp_deidentify_date_shift]
        public static object DeidDateShift(
            string projectId,
            string inputCsvFile,
            string outputCsvFile,
            int lowerBoundDays,
            int upperBoundDays,
            string dateFields,
            string contextField = "",
            string keyName = "",
            string wrappedKey = "")
        {
            var dlp = DlpServiceClient.Create();

            // Read file
            string[] csvLines = File.ReadAllLines(inputCsvFile);
            string[] csvHeaders = csvLines[0].Split(',');
            string[] csvRows = csvLines.Skip(1).ToArray();

            // Convert dates to protobuf format, and everything else to a string
            var protoHeaders = csvHeaders.Select(header => new FieldId { Name = header });
            var protoRows = csvRows.Select(CsvRow =>
            {
                var rowValues = CsvRow.Split(',');
                var protoValues = rowValues.Select(RowValue =>
                {
                    System.DateTime parsedDate;
                    if (System.DateTime.TryParse(RowValue, out parsedDate))
                    {
                        return new Value
                        {
                            DateValue = new Google.Type.Date
                            {
                                Year = parsedDate.Year,
                                Month = parsedDate.Month,
                                Day = parsedDate.Day
                            }
                        };
                    }
                    else
                    {
                        return new Value
                        {
                            StringValue = RowValue
                        };
                    }
                });

                var rowObject = new Table.Types.Row();
                rowObject.Values.Add(protoValues);
                return rowObject;
            });

            var dateFieldList = dateFields
                .Split(',')
                .Select(field => new FieldId { Name = field });

            // Construct + execute the request
            var dateShiftConfig = new DateShiftConfig
            {
                LowerBoundDays = lowerBoundDays,
                UpperBoundDays = upperBoundDays
            };
            bool hasKeyName = !String.IsNullOrEmpty(keyName);
            bool hasWrappedKey = !String.IsNullOrEmpty(wrappedKey);
            bool hasContext = !String.IsNullOrEmpty(contextField);
            if (hasKeyName && hasWrappedKey && hasContext)
            {
                dateShiftConfig.Context = new FieldId { Name = contextField };
                dateShiftConfig.CryptoKey = new CryptoKey
                {
                    KmsWrapped = new KmsWrappedCryptoKey
                    {
                        WrappedKey = ByteString.FromBase64(wrappedKey),
                        CryptoKeyName = keyName
                    }
                };
            }
            else if (hasKeyName || hasWrappedKey || hasContext)
            {
                throw new ArgumentException("Must specify ALL or NONE of: {contextFieldId, keyName, wrappedKey}!");
            }

            var deidConfig = new DeidentifyConfig
            {
                RecordTransformations = new RecordTransformations
                {
                    FieldTransformations =
                    {
                        new FieldTransformation
                        {
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                DateShiftConfig = dateShiftConfig
                            },
                            Fields = { dateFieldList }
                        }
                    }
                }
            };

            DeidentifyContentResponse response = dlp.DeidentifyContent(
                new DeidentifyContentRequest
                {
                    Parent = $"projects/{projectId}",
                    DeidentifyConfig = deidConfig,
                    Item = new ContentItem
                    {
                        Table = new Table
                        {
                            Headers = { protoHeaders },
                            Rows = { protoRows }
                        }
                    }
                });

            // Save the results
            List<String> outputLines = new List<string>();
            outputLines.Add(csvLines[0]);

            outputLines.AddRange(response.Item.Table.Rows.Select(ProtoRow =>
            {
                var Values = ProtoRow.Values.Select(ProtoValue =>
                {
                    if (ProtoValue.DateValue != null)
                    {
                        var ProtoDate = ProtoValue.DateValue;
                        System.DateTime Date = new System.DateTime(
                            ProtoDate.Year, ProtoDate.Month, ProtoDate.Day);
                        return Date.ToShortDateString();
                    }
                    else
                    {
                        return ProtoValue.StringValue;
                    }
                });
                return String.Join(',', Values);
            }));

            File.WriteAllLines(outputCsvFile, outputLines);

            return 0;
        }
        // [END dlp_deidentify_date_shift]
    }
}
