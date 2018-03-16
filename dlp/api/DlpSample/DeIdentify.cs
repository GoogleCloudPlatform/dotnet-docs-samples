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
            string WrappedKeyFile,
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
                                            WrappedKey = ByteString.CopyFrom(File.ReadAllBytes(WrappedKeyFile))
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
            string Value,
            string KeyName,
            string WrappedKeyFile,
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
                                            WrappedKey = ByteString.CopyFrom(File.ReadAllBytes(WrappedKeyFile))
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
                Item = new ContentItem { Value = Value }
            });
            Console.WriteLine($"Reidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_reidentify_fpe]
    }
}
