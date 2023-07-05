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

// [START dlp_reidentify_table_fpe]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using static Google.Cloud.Dlp.V2.CryptoReplaceFfxFpeConfig.Types;

public class ReidentifyTableDataWithFpe
{
    public static Table ReidentifyTableData(
        string projectId,
        string keyName,
        string wrappedKey,
        FfxCommonNativeAlphabet alphabet = FfxCommonNativeAlphabet.Numeric,
        Table tableToInspect = null)
    {
        // Instantiate a client.
        var dlp = DlpServiceClient.Create();

        // Construct the table if null.
        var table = tableToInspect;

        if (table == null)
        {
            var row1 = new Value[]
            {
                new Value { StringValue = "28777" },
                new Value { StringValue = "Justin" }
            };
            var row2 = new Value[]
            {
                new Value { StringValue = "28778" },
                new Value { StringValue = "Gary" }
            };

            table = new Table
            {
                Headers =
                {
                    new FieldId { Name = "Employee ID" },
                    new FieldId { Name = "Employee Name" }
                },
                Rows =
                {
                    new Table.Types.Row { Values = { row1 } },
                    new Table.Types.Row { Values = { row2 } }
                }
            };
        }

        // Construct the content item by providing the table.
        var contentItem = new ContentItem { Table = table };

        // Specify how to decrypt the previously de-identified information.
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

        // Specify the field to be decrypted.
        var fields = new FieldId[] { new FieldId { Name = "Employee ID" } };

        // Construct the re-identify config and specify the transformation.
        var reidentifyConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            CryptoReplaceFfxFpeConfig = cryptoReplaceFfxFpeConfig,
                        },
                        Fields = { fields }
                    }
                }
            }
        };

        // Construct the request.
        var request = new ReidentifyContentRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            Item = contentItem,
            ReidentifyConfig = reidentifyConfig
        };

        // Call the API.
        ReidentifyContentResponse response = dlp.ReidentifyContent(request);

        // Inspect the response.
        Console.WriteLine($"Table after re-identification: {response.Item.Table}");

        return response.Item.Table;
    }
}

// [END dlp_reidentify_table_fpe]
