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

// [START dlp_reidentify_free_text_with_fpe_using_surrogate]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

public class ReidentifyFreeTextWithFpeUsingSurrogate
{
    public static ReidentifyContentResponse ReidentifyFreeText(
        string projectId,
        string text,
        string unwrappedKey,
        FfxCommonNativeAlphabet alphabet = FfxCommonNativeAlphabet.Numeric,
        InfoType surrogateType = null)
    {
        // Instantiate dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the infoType which will be used as surrogate type and infoType both.
        var infoType = surrogateType ?? new InfoType { Name = "PHONE_TOKEN" };

        // Construct crypto replace config using crypto key name and wrapped key.
        var cryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
        {
            CryptoKey = new CryptoKey
            {
                Unwrapped = new UnwrappedCryptoKey
                {
                    Key = ByteString.FromBase64(unwrappedKey)
                }
            },
            CommonAlphabet = alphabet,
            SurrogateInfoType = infoType
        };

        // Construct the re-identify config using crypto replace config.
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
                            CryptoReplaceFfxFpeConfig = cryptoReplaceFfxFpeConfig
                        }
                    }
                }
            }
        };

        // Construct the inspect config with the help of custom info type.
        var inspectConfig = new InspectConfig
        {
            CustomInfoTypes =
            {
                new CustomInfoType
                {
                    InfoType = infoType,
                    SurrogateType = new CustomInfoType.Types.SurrogateType()
                }
            }
        };

        // Construct the request.
        var request = new ReidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            ReidentifyConfig = reidentifyConfig,
            InspectConfig = inspectConfig,
            Item = new ContentItem { Value = text }
        };

        // Call the API.
        ReidentifyContentResponse response = dlp.ReidentifyContent(request);

        // Inspect the response.
        Console.WriteLine($"Re-identified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_reidentify_free_text_with_fpe_using_surrogate]
