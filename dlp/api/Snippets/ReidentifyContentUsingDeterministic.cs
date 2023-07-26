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

// [START dlp_reidentify_deterministic]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;

public class ReidentifyContentUsingDeterministic
{
    public static ReidentifyContentResponse ReidentifyDeterministic(
        string projectId,
        string text,
        string keyName,
        string wrappedKey,
        InfoType surrogateType = null)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Construct the infoType if null which will be used as surrogate type and infoType both.
        var infoType = surrogateType ?? new InfoType { Name = "PHONE_TOKEN" };

        // Construct crypto deterministic config using the crypto key name and wrapped key.
        var cryptoDeterministicConfig = new CryptoDeterministicConfig
        {
            CryptoKey = new CryptoKey
            {
                KmsWrapped = new KmsWrappedCryptoKey
                {
                    CryptoKeyName = keyName,
                    WrappedKey = ByteString.FromBase64(wrappedKey)
                }
            },
            SurrogateInfoType = infoType,
        };

        // Construct the reidentify config using crypto config.
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
                            CryptoDeterministicConfig = cryptoDeterministicConfig
                        }
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
            Item = new ContentItem { Value = text },
        };

        // Call the API.
        ReidentifyContentResponse response = dlp.ReidentifyContent(request);

        // Inspect the response.
        Console.WriteLine($"Reidentified content: {response.Item.Value}");
        return response;
    }
}

// [END dlp_reidentify_deterministic]
