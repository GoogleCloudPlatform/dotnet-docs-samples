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

// [START dlp_deidentify_free_text_with_fpe_using_surrogate]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

public class DeidentifyFreeTextWithFpeUsingSurrogate
{
    public static DeidentifyContentResponse Deidentify(
        string projectId,
        string text,
        string unwrappedKey,
        IEnumerable<InfoType> infoTypes = null,
        FfxCommonNativeAlphabet alphabet = FfxCommonNativeAlphabet.Numeric,
        InfoType surrogateType = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Specify the type of info to be inspected and construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes =
            {
                infoTypes ?? new InfoType[]
                {
                    new InfoType { Name = "PHONE_NUMBER" }
                }
            },
            MinLikelihood = Likelihood.Unlikely
        };

        // Construct the crypto replace ffxfpe config by providing the unwrapped crypto key.
        var cryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
        {
            CommonAlphabet = alphabet,
            CryptoKey = new CryptoKey
            {
                Unwrapped = new UnwrappedCryptoKey
                {
                    Key = ByteString.FromBase64(unwrappedKey)
                }
            },
            SurrogateInfoType = surrogateType ?? new InfoType { Name = "PHONE_TOKEN" }
        };

        // Construct the deidentify config using crypto config created above.
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
                            CryptoReplaceFfxFpeConfig = cryptoReplaceFfxFpeConfig
                        }
                    }
                }
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifyConfig,
            InspectConfig = inspectConfig,
            Item = new ContentItem { Value = text }
        };

        // Call the API.
        DeidentifyContentResponse response = dlp.DeidentifyContent(request);

        // Check the de-identified content.
        Console.WriteLine($"De-identified content: {response.Item.Value}");
        return response;
    }
}


// [END dlp_deidentify_free_text_with_fpe_using_surrogate]