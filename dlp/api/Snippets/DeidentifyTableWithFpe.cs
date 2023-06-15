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

// [START dlp_deidentify_table_fpe]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

public class DeidentifyTableWithFpe
{
    public static Table DeidentifyTable(
        string projectId,
        string keyName,
        string wrappedKey,
        FfxCommonNativeAlphabet alphabet = FfxCommonNativeAlphabet.Numeric,
        Table tableToInspect = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        if (tableToInspect == null)
        {
            var row1 = new Value[]
            {
                new Value { StringValue = "11111" },
                new Value { StringValue = "2015" },
                new Value { StringValue = "$10" }
            };
            var row2 = new Value[]
            {
                new Value { StringValue = "33333" },
                new Value { StringValue = "2016" },
                new Value { StringValue = "$20" }
            };
            var row3 = new Value[]
            {
                new Value { StringValue = "22222" },
                new Value { StringValue = "2016" },
                new Value { StringValue = "$15" }
            };

            tableToInspect = new Table
            {
                Headers =
                {
                    new FieldId { Name = "Employee ID" },
                    new FieldId { Name = "Date" },
                    new FieldId { Name = "Compensation" }
                },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } },
                    new Table.Types.Row { Values = { row3 } }
                }
            };
        }

        // Provide the table and construct the content item.
        var contentItem = new ContentItem { Table = tableToInspect };

        // Specify an encrypted AES-256 key and the name of the Cloud KMS Key that
        // encrypted it and specify how it should be encrypted.
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
            CommonAlphabet = alphabet
        };

        // Specify fields to be encrypted.
        var fields = new FieldId[] { new FieldId { Name = "Employee ID" } };

        // Construct the deidentify config using crypto replace config created above.
        var deidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            CryptoReplaceFfxFpeConfig = cryptoReplaceFfxFpeConfig
                        },
                        Fields = { fields }
                    }
                }
            }
        };

        // Construct the request.
        var request = new DeidentifyContentRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            DeidentifyConfig = deidentifyConfig,
            Item = contentItem,
        };

        // Call the API.
        DeidentifyContentResponse response = dlp.DeidentifyContent(request);

        // Inspect the response.
        Console.WriteLine(response.Item.Table);

        return response.Item.Table;
    }
}

// [END dlp_deidentify_table_fpe]
