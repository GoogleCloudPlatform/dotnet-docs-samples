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

// [START dlp_deidentify_fpe]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

public class DeidentifyWithFpe
{
    public static DeidentifyContentResponse Deidentify(
        string projectId,
        string dataValue,
        IEnumerable<InfoType> infoTypes,
        string keyName,
        string wrappedKey,
        FfxCommonNativeAlphabet alphabet)
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
                                CommonAlphabet = alphabet,
                                CryptoKey = new CryptoKey
                                {
                                    KmsWrapped = new KmsWrappedCryptoKey
                                    {
                                        CryptoKeyName = keyName,
                                        WrappedKey = ByteString.FromBase64 (wrappedKey)
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

        var dlp = DlpServiceClient.Create();
        var response = dlp.DeidentifyContent(
            new DeidentifyContentRequest
            {
                Parent = new LocationName(projectId, "global").ToString(),
                InspectConfig = new InspectConfig
                {
                    InfoTypes = { infoTypes }
                },
                DeidentifyConfig = deidentifyConfig,
                Item = new ContentItem { Value = dataValue }
            });

        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_deidentify_fpe]
