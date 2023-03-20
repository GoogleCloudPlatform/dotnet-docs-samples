/*
 * Copyright 2023 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START bigquerystorage_quickstart]

using Avro;
using Avro.IO;
using Avro.Specific;
using BigQueryStorage.Samples.Utilities;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.BigQuery.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Google.Cloud.BigQuery.Storage.V1.ReadSession.Types;

public class QuickstartSample
{

    public async Task<List<BabyNamesData>> QuickstartAsync(string projectId)
    {
        var bigQueryReadClient = BigQueryReadClient.Create();
        CreateReadSessionRequest createReadSessionRequest = new CreateReadSessionRequest
        {
            ParentAsProjectName = new ProjectName(projectId),
            ReadSession = new ReadSession
            {
                // This example uses baby name data from the public datasets.
                TableAsTableName = new TableName("bigquery-public-data", "usa_names", "usa_1910_current"),
                DataFormat = DataFormat.Avro,
                ReadOptions = new TableReadOptions
                {
                    // Specify the columns to be projected by adding them to the selected fields.
                    SelectedFields = { "name", "number", "state" },
                    RowRestriction = "state = \"WA\"",
                },
            },
            // Sets maximum number of reading streams to 1.
            MaxStreamCount = 1,
        };
        var readSession = bigQueryReadClient.CreateReadSession(createReadSessionRequest);

        // Uses the first (and only) stream to read data from and reading starts from offset 0.
        var readRowsStream = bigQueryReadClient.ReadRows(readSession.Streams.First().Name, 0).GetResponseStream();
        var schema = Schema.Parse(readSession.AvroSchema.Schema);

        // BabyNamesData has been generated using AvroGen, version 1.11.1.
        // The file is available here https://github.com/GoogleCloudPlatform/dotnet-docs-samples/blob/main/bigquery-storage/api/BigQueryStorage.Samples/Utilities/BabyNamesData.g.cs
        var reader = new SpecificDatumReader<BabyNamesData>(schema, schema);
        var dataList = new List<BabyNamesData>();
        await foreach (var readRowResponse in readRowsStream)
        {
            var byteArray = readRowResponse.AvroRows.SerializedBinaryRows.ToByteArray();
            var decoder = new BinaryDecoder(new MemoryStream(byteArray));
            for (int row = 0; row < readRowResponse.RowCount; row++)
            {
                var record = reader.Read(new BabyNamesData(), decoder);
                dataList.Add(record);
                Console.WriteLine($"name: {record.name}, state: {record.state}, number: {record.number}");
            }
        }
        return dataList;
    }
}
// [END bigquerystorage_quickstart]