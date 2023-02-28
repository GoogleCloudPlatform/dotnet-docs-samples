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
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Google.Cloud.BigQuery.Storage.V1.BigQueryReadClient;
using static Google.Cloud.BigQuery.Storage.V1.ReadSession.Types;

public class StorageAvroSample
{
    public struct Data
    {
        public string Name { get; set; }
        public string State { get; set; }
        public long Number { get; set; }
    }
    public async Task<List<Data>> AvroSampleAsync(string ProjectId)
    {
        BigQueryReadClient bigQueryReadClient = BigQueryReadClient.Create();

        // This example uses baby name data from the public datasets.
        var srcTable = "projects/bigquery-public-data/datasets/usa_names/tables/usa_1910_current";
        var parent = $"projects/{ProjectId}";

        // Specify the columns to be projected by adding them to the selected fields.
        TableReadOptions tableReadOptions = new TableReadOptions
        {
            SelectedFields = { "name", "number", "state" },
            RowRestriction = "state = \"WA\""
        };

        // Specifications for the read session we want to be created.
        ReadSession readSession = new ReadSession
        {
            Table = srcTable,
            DataFormat = DataFormat.Avro,
            ReadOptions = tableReadOptions
        };

        CreateReadSessionRequest createReadSessionRequest = new CreateReadSessionRequest
        {
            Parent = parent,
            ReadSession = readSession,
            MaxStreamCount = 1,
        };
        ReadSession session = bigQueryReadClient.CreateReadSession(createReadSessionRequest);

        var schema = Schema.Parse(session.AvroSchema.Schema);
        DatumReader<GenericRecord> datumReader = new GenericDatumReader<GenericRecord>(schema, schema);

        // Using the first stream to perform reading.
        string streamName = session.Streams.First().Name;

        ReadRowsRequest readRowsRequest = new ReadRowsRequest
        {
            ReadStream = streamName
        };
        ReadRowsStream rowsStream = bigQueryReadClient.ReadRows(readRowsRequest);
        List<Data> dataList = new List<Data>();

        // Process each block of rows as they arrive and decode them.
        while (await rowsStream.GrpcCall.ResponseStream.MoveNext().ConfigureAwait(false))
        {
            var curr = rowsStream.GrpcCall.ResponseStream.Current;
            var decoder = new BinaryDecoder(new MemoryStream(curr.AvroRows.SerializedBinaryRows.ToByteArray()));
            GenericRecord row = null;
            do
            {
                try
                {
                    // Raise AvroException when end of stream is reached.
                    row = datumReader.Read(null, decoder);
                    row.TryGetValue("name", out var name);
                    row.TryGetValue("state", out var state);
                    row.TryGetValue("number", out var number);
                    dataList.Add(new Data { Name = (string)name, Number = (long)number, State = (string)state });
                    Console.WriteLine($"name: {name}, state: {state}, number: {number}");
                }
                catch (AvroException ex)
                {
                    row = null;
                }
            } while (row != null);
        }
        return dataList;
    }
}
// [END bigquerystorage_quickstart]