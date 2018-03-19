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
    public class DeIdentify : DlpSampleBase
    {
        // [START dlp_deidentify_masking]
        public static object DeidMask(
            string ProjectId,
            string DataValue,
            string InfoTypes,
            string MaskingCharacter,
            int NumberToMask,
            bool ReverseOrder)
        {
            DeidentifyContentRequest request = new DeidentifyContentRequest
            {
                ParentAsProjectName = new ProjectName(ProjectId),
                DeidentifyConfig = new DeidentifyConfig
                {
                    InfoTypeTransformations = new InfoTypeTransformations {
                        Transformations = {
                            new InfoTypeTransformations.Types.InfoTypeTransformation
                            {
                                PrimitiveTransformation = new PrimitiveTransformation
                                {
                                    CharacterMaskConfig = new CharacterMaskConfig
                                    {
                                        MaskingCharacter = MaskingCharacter,
                                        NumberToMask = NumberToMask,
                                        ReverseOrder = ReverseOrder
                                    }
                                }
                            }
                        }
                    }
                },
                Item = new ContentItem
                {
                    Value = DataValue
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
            string ProjectId,
            string DataValue,
            string KeyName,
            string WrappedKey,
            string Alphabet)
        {
            var DeidentifyConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations {
                    Transformations = {
                        new InfoTypeTransformations.Types.InfoTypeTransformation
                        {
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                CryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
                                {
                                    CommonAlphabet = (FfxCommonNativeAlphabet) Enum.Parse(typeof(FfxCommonNativeAlphabet), Alphabet),
                                    CryptoKey = new CryptoKey
                                    {
                                        KmsWrapped = new KmsWrappedCryptoKey
                                        {
                                            CryptoKeyName = KeyName,
                                            WrappedKey = ByteString.FromBase64(WrappedKey)
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
            var response = dlp.DeidentifyContent(new DeidentifyContentRequest
            {
                ParentAsProjectName = new ProjectName(ProjectId),
                DeidentifyConfig = DeidentifyConfig,
                Item = new ContentItem { Value = DataValue }
            });
            Console.WriteLine($"Deidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_deidentify_fpe]

        // [START dlp_reidentify_fpe]
        public static object ReidFpe(
            string ProjectId,
            string DataValue,
            string KeyName,
            string WrappedKey,
            string Alphabet)
        {
            var ReidentifyConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations {
                    Transformations = {
                        new InfoTypeTransformations.Types.InfoTypeTransformation
                        {
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                CryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
                                {
                                    CommonAlphabet = (FfxCommonNativeAlphabet) Enum.Parse(typeof(FfxCommonNativeAlphabet), Alphabet),
                                    CryptoKey = new CryptoKey
                                    {
                                        KmsWrapped = new KmsWrappedCryptoKey
                                        {
                                            CryptoKeyName = KeyName,
                                            WrappedKey = ByteString.FromBase64(WrappedKey)
                                        }
                                    },
                                    SurrogateInfoType = new InfoType
                                    {
                                        Name = "TOKEN"
                                    }
                                }
                            },
                            InfoTypes = {
                                new InfoType { Name = "TOKEN" }
                            }
                        }
                    }
                }
            };

            var InspectConfig = new InspectConfig {
                CustomInfoTypes = {
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

            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.ReidentifyContent(new ReidentifyContentRequest
            {
                ParentAsProjectName = new ProjectName(ProjectId),
                InspectConfig = InspectConfig,
                ReidentifyConfig = ReidentifyConfig,
                Item = new ContentItem { Value = DataValue }
            });
            Console.WriteLine($"Reidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_reidentify_fpe]

        // [START dlp_deidentify_date_shift]
        public static object DeidDateShift(
            string ProjectId,
            string InputCsvFile,
            string OutputCsvFile,
            int LowerBoundDays,
            int UpperBoundDays,
            string DateFields,
            string ContextField = "",
            string KeyName = "",
            string WrappedKey = "")
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Read file
            string[] CsvLines = File.ReadAllLines(InputCsvFile);
            string[] CsvHeaders = CsvLines[0].Split(',');
            string[] CsvRows = CsvLines.Skip(1).ToArray();

            // Convert dates to protobuf format, and everything else to a string
            var ProtoHeaders = CsvHeaders.Select(header => new FieldId { Name = header });
            var ProtoRows = CsvRows.Select(CsvRow =>
            {
                var RowValues = CsvRow.Split(',');
                var ProtoValues = RowValues.Select(RowValue =>
                {
                    System.DateTime ParsedDate;
                    if (System.DateTime.TryParse(RowValue, out ParsedDate))
                    {
                        return new Value
                        {
                            DateValue = new Google.Type.Date
                            {
                                Year = ParsedDate.Year,
                                Month = ParsedDate.Month,
                                Day = ParsedDate.Day
                            }
                        };
                    } else {
                        return new Value
                        {
                            StringValue = RowValue
                        };
                    }
                });

                var RowObject = new Table.Types.Row();
                RowObject.Values.Add(ProtoValues);
                return RowObject;
            });

            var DateFieldList = DateFields
                                 .Split(',')
                                 .Select(field => new FieldId { Name = field });

            // Construct + execute the request
            DateShiftConfig DateShiftConfig = new DateShiftConfig
            {
                LowerBoundDays = LowerBoundDays,
                UpperBoundDays = UpperBoundDays
            };
            bool hasKeyName = !String.IsNullOrEmpty(KeyName);
            bool hasWrappedKey = !String.IsNullOrEmpty(WrappedKey);
            bool hasContext = !String.IsNullOrEmpty(ContextField);
            if (hasKeyName && hasWrappedKey && hasContext)
            {
                DateShiftConfig.Context = new FieldId { Name = ContextField };
                DateShiftConfig.CryptoKey = new CryptoKey
                {
                    KmsWrapped = new KmsWrappedCryptoKey
                    {
                        WrappedKey = ByteString.FromBase64(WrappedKey),
                        CryptoKeyName = KeyName
                    }
                };
            }
            else if (hasKeyName || hasWrappedKey || hasContext)
            {
                throw new ArgumentException("Must specify ALL or NONE of: {contextFieldId, keyName, wrappedKey}!");
            }

            DeidentifyConfig deidConfig = new DeidentifyConfig
            {
                RecordTransformations = new RecordTransformations
                {
                    FieldTransformations =
                    {
                        new FieldTransformation
                        {
                            PrimitiveTransformation = new PrimitiveTransformation
                            {
                                DateShiftConfig = DateShiftConfig
                            },
                            Fields = { DateFieldList }
                        }
                    }
                }
            };

            var response = dlp.DeidentifyContent(new DeidentifyContentRequest
            {
                Parent = $"projects/{ProjectId}",
                DeidentifyConfig = deidConfig,
                Item = new ContentItem
                {
                    Table = new Table
                    {
                        Headers = { ProtoHeaders },
                        Rows = { ProtoRows }
                    }
                }
            });

            // Save the results
            List<String> OutputLines = new List<string>();
            OutputLines.Add(CsvLines[0]);

            OutputLines.AddRange(response.Item.Table.Rows.Select(ProtoRow => {
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

            File.WriteAllLines(OutputCsvFile, OutputLines);

            return 0;
        }
        // [END dlp_deidentify_date_shift]
    }
}
