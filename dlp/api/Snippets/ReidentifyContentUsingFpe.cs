// Copyright 2023 Google Inc.
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

// [START dlp_reidentify_fpe]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

public class ReidentifyContentUsingFpe
{
    public static ReidentifyContentResponse ReidentifyContent(
        string projectId,
        string text,
        string keyName,
        string wrappedKey,
        FfxCommonNativeAlphabet alphabet = FfxCommonNativeAlphabet.Numeric,
        InfoType surrogateType = null)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Specify the type of info the inspection will re-identify. This must use the same custom
        // info type that was used as a surrogate during the initial encryption.
        var infotype = surrogateType ?? new InfoType { Name = "SSN_TOKEN" };

        // Specify how to un-encrypt the previously de-identified information.
        var cryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
        {
            CryptoKey = new CryptoKey
            {
                KmsWrapped = new KmsWrappedCryptoKey
                {
                    CryptoKeyName = keyName,
                    WrappedKey = ByteString.FromBase64(wrappedKey)
                }
            },
            CommonAlphabet = alphabet,
            SurrogateInfoType = infotype
        };

        // Construct reidentify config using above created crypto replace config.
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
                            CryptoReplaceFfxFpeConfig = cryptoReplaceFfxFpeConfig,
                        },
                        InfoTypes = { infotype }
                    }
                }
            }
        };

        // Construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            CustomInfoTypes =
            {
                new CustomInfoType
                {
                    InfoType = infotype,
                    SurrogateType = new CustomInfoType.Types.SurrogateType()
                }
            }
        };

        // Construct the request.
        var request = new ReidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectConfig = inspectConfig,
            ReidentifyConfig = reidentifyConfig,
            Item = new ContentItem { Value = text },
        };

        // Call the API.
        var response = dlp.ReidentifyContent(request);

        // Inspect the response.
        Console.WriteLine($"Reidentified content: {response.Item.Value}");

        return response;
    }
}

// [END dlp_reidentify_fpe]
