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

// [START dlp_deidentify_date_shift]

using System;
using System.IO;
using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Protobuf;

public class DeidentifyWithDateShift
{
    public static DeidentifyContentResponse Deidentify(
        string projectId,
        string inputCsvFilePath,
        int lowerBoundDays,
        int upperBoundDays,
        string dateFields,
        string contextField,
        string keyName,
        string wrappedKey)
    {
        var hasKeyName = !string.IsNullOrEmpty(keyName);
        var hasWrappedKey = !string.IsNullOrEmpty(wrappedKey);
        var hasContext = !string.IsNullOrEmpty(contextField);
        bool allFieldsSet = hasKeyName && hasWrappedKey && hasContext;
        bool noFieldsSet = !hasKeyName && !hasWrappedKey && !hasContext;
        if (!(allFieldsSet || noFieldsSet))
        {
            throw new ArgumentException("Must specify ALL or NONE of: {contextFieldId, keyName, wrappedKey}!");
        }

        var dlp = DlpServiceClient.Create();

        // Read file
        var csvLines = File.ReadAllLines(inputCsvFilePath);
        var csvHeaders = csvLines[0].Split(',');
        var csvRows = csvLines.Skip(1).ToArray();

        // Convert dates to protobuf format, and everything else to a string
        var protoHeaders = csvHeaders.Select(header => new FieldId { Name = header });
        var protoRows = csvRows.Select(csvRow =>
        {
            var rowValues = csvRow.Split(',');
            var protoValues = rowValues.Select(rowValue =>
               System.DateTime.TryParse(rowValue, out var parsedDate)
               ? new Value { DateValue = Google.Type.Date.FromDateTime(parsedDate) }
               : new Value { StringValue = rowValue });

            var rowObject = new Table.Types.Row();
            rowObject.Values.Add(protoValues);
            return rowObject;
        });

        var dateFieldList = dateFields
            .Split(',')
            .Select(field => new FieldId { Name = field });

        // Construct + execute the request
        var dateShiftConfig = new DateShiftConfig
        {
            LowerBoundDays = lowerBoundDays,
            UpperBoundDays = upperBoundDays
        };

        dateShiftConfig.Context = new FieldId { Name = contextField };
        dateShiftConfig.CryptoKey = new CryptoKey
        {
            KmsWrapped = new KmsWrappedCryptoKey
            {
                WrappedKey = ByteString.FromBase64(wrappedKey),
                CryptoKeyName = keyName
            }
        };

        var deidConfig = new DeidentifyConfig
        {
            RecordTransformations = new RecordTransformations
            {
                FieldTransformations =
                {
                    new FieldTransformation
                    {
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            DateShiftConfig = dateShiftConfig
                        },
                        Fields = { dateFieldList }
                    }
                }
            }
        };

        var response = dlp.DeidentifyContent(
            new DeidentifyContentRequest
            {
                Parent = new LocationName(projectId, "global").ToString(),
                DeidentifyConfig = deidConfig,
                Item = new ContentItem
                {
                    Table = new Table
                    {
                        Headers = { protoHeaders },
                        Rows = { protoRows }
                    }
                }
            });

        return response;
    }
}
// [END dlp_deidentify_date_shift]
