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

// [START dlp_deidentify_deterministic]

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class DeidentifyWithDeterministic
{
    public static DeidentifyContentResponse Deidentify(
        string projectId,
        string text,
        string keyName,
        string wrapperKey,
        InfoType surrogateType = null,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct the inspect config by specifying the type of info to be inspected.
        var inspectConfig = new InspectConfig
        {
            InfoTypes =
            {
                infoTypes ?? new InfoType[] { new InfoType { Name = "PHONE_NUMBER" } }
            }
        };

        // Construct the crypto deterministic config by providing key name, wrapped key and surrogate type.
        var cryptoDeterministicConfig = new CryptoDeterministicConfig
        {
            CryptoKey = new CryptoKey
            {
                KmsWrapped = new KmsWrappedCryptoKey
                {
                    CryptoKeyName = keyName,
                    WrappedKey = Google.Protobuf.ByteString.FromBase64(wrapperKey)
                }
            },
            SurrogateInfoType = surrogateType ?? new InfoType { Name = "PHONE_TOKEN" }
        };


        // Construct the deidentify config using crypto deterministic config.
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
                            CryptoDeterministicConfig = cryptoDeterministicConfig
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

// [END dlp_deidentify_deterministic]
