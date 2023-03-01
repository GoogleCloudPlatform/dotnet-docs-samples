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
using Avro.Generic;
using Avro.IO;
using Google.Cloud.BigQuery.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Google.Cloud.BigQuery.Storage.V1.ReadSession.Types;

public class QuickstartSample
{
    public struct Data
    {
        public string Name { get; set; }
        public string State { get; set; }
        public long Number { get; set; }
    }

    public async Task<List<Data>> QuickstartAsync(string projectId)
    {
        BigQueryReadClient bigQueryReadClient = BigQueryReadClient.Create();

        // This example uses baby name data from the public datasets.
        var srcTable = "projects/bigquery-public-data/datasets/usa_names/tables/usa_1910_current";
        var parent = $"projects/{projectId}";

        CreateReadSessionRequest createReadSessionRequest = new CreateReadSessionRequest
        {
            Parent = parent,
            ReadSession = new ReadSession
            {
                Table = srcTable,
                DataFormat = DataFormat.Avro,
                ReadOptions = new TableReadOptions
                {
                    // Specify the columns to be projected by adding them to the selected fields.
                    SelectedFields = { "name", "number", "state" },
                    RowRestriction = "state = \"WA\""
                }
            },
            MaxStreamCount = 1,
        };
        var session = bigQueryReadClient.CreateReadSession(createReadSessionRequest);

        var schema = Schema.Parse(session.AvroSchema.Schema);
        DatumReader<GenericRecord> reader = new GenericDatumReader<GenericRecord>(schema, schema);

        // Using the first stream to perform reading.
        var readRowStream = bigQueryReadClient.ReadRows(session.Streams.First().Name, 0).GetResponseStream();
        var dataList = new List<Data>();

        await foreach (var stream in readRowStream)
        {
            var byteArray = stream.AvroRows.SerializedBinaryRows.ToByteArray();
            var decoder = new BinaryDecoder(new MemoryStream(byteArray));

            for (int i = 0; i < stream.RowCount; i++)
            {
                var row = reader.Read(null!, decoder);
                row.TryGetValue("name", out var name);
                row.TryGetValue("state", out var state);
                row.TryGetValue("number", out var number);
                dataList.Add(new Data { Name = (string) name, Number = (long) number, State = (string) state });
                Console.WriteLine($"name: {name}, state: {state}, number: {number}");
            }
        }
        return dataList;
    }
}
// [END bigquerystorage_quickstart]