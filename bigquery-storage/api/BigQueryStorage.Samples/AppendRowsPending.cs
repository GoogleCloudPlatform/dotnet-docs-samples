/*
 * Copyright 2022 Google LLC
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

// [START bigquerystorage_append_rows_pending]

using Google.Api.Gax.Grpc;
using Google.Cloud.BigQuery.Storage.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Google.Cloud.BigQuery.Storage.V1.AppendRowsRequest.Types;

public class AppendRowsPendingSample
{
    /// <summary>
    /// This code sample demonstrates how to write records in pending mode.
    /// Create a write stream, write some sample data, and commit the stream to append the rows.
    /// The CustomerRecord proto used in the sample can be seen in Resources folder and generated C# is placed in Data folder in
    /// https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/main/bigquery-storage/api/BigQueryStorage.Samples
    /// </summary>
    public async Task AppendRowsPendingAsync(string projectId, string datasetId, string tableId)
    {
        BigQueryWriteClient bigQueryWriteClient = await BigQueryWriteClient.CreateAsync();
        // Initialize a write stream for the specified table.
        // When creating the stream, choose the type. Use the Pending type to wait
        // until the stream is committed before it is visible. See:
        // https://cloud.google.com/bigquery/docs/reference/storage/rpc/google.cloud.bigquery.storage.v1#google.cloud.bigquery.storage.v1.WriteStream.Type
        WriteStream stream = new WriteStream { Type = WriteStream.Types.Type.Pending };
        TableName tableName = TableName.FromProjectDatasetTable(projectId, datasetId, tableId);

        stream = await bigQueryWriteClient.CreateWriteStreamAsync(tableName, stream);

        // Initialize streaming call, retrieving the stream object
        BigQueryWriteClient.AppendRowsStream rowAppender = bigQueryWriteClient.AppendRows();

        // Sending requests and retrieving responses can be arbitrarily interleaved.
        // Exact sequence will depend on client/server behavior.
        // Create task to do something with responses from server.
        Task appendResultsHandlerTask = Task.Run(async () =>
        {
            AsyncResponseStream<AppendRowsResponse> appendRowResults = rowAppender.GetResponseStream();
            while (await appendRowResults.MoveNextAsync())
            {
                AppendRowsResponse responseItem = appendRowResults.Current;
                // Do something with responses.
                Console.WriteLine($"Appending rows resulted in: {responseItem.AppendResult}");
            }
            // The response stream has completed.
        });

        // List of records to be appended in the table.
        List<CustomerRecord> records = new List<CustomerRecord>
        {
            new CustomerRecord { CustomerNumber = 1, CustomerName = "Alice" },
            new CustomerRecord { CustomerNumber = 2, CustomerName = "Bob" }
        };

        // Create a batch of row data by appending serialized bytes to the
        // SerializedRows repeated field.
        ProtoData protoData = new ProtoData
        {
            WriterSchema = new ProtoSchema { ProtoDescriptor = CustomerRecord.Descriptor.ToProto() },
            Rows = new ProtoRows { SerializedRows = { records.Select(r => r.ToByteString()) } }
        };

        // Initialize the append row request.
        AppendRowsRequest appendRowRequest = new AppendRowsRequest
        {
            WriteStreamAsWriteStreamName = stream.WriteStreamName,
            ProtoRows = protoData
        };

        // Stream a request to the server.
        await rowAppender.WriteAsync(appendRowRequest);

        // Append a second batch of data.
        protoData = new ProtoData
        {
            Rows = new ProtoRows { SerializedRows = { new CustomerRecord { CustomerNumber = 3, CustomerName = "Charles" }.ToByteString() } }
        };

        // Since this is the second request, you only need to include the row data.
        // The name of the stream and protocol buffers descriptor is only needed in
        // the first request.
        appendRowRequest = new AppendRowsRequest
        {
            // If Offset is not present, the write is performed at the current end of stream.
            ProtoRows = protoData
        };

        await rowAppender.WriteAsync(appendRowRequest);

        // Complete writing requests to the stream.
        await rowAppender.WriteCompleteAsync();

        // Await the handler. This will complete once all server responses have been processed.
        await appendResultsHandlerTask;

        // A Pending type stream must be "finalized" before being committed. No new
        // records can be written to the stream after this method has been called.
        await bigQueryWriteClient.FinalizeWriteStreamAsync(stream.Name);
        BatchCommitWriteStreamsRequest batchCommitWriteStreamsRequest = new BatchCommitWriteStreamsRequest
        {
            Parent = tableName.ToString(),
            WriteStreams = { stream.Name }
        };

        BatchCommitWriteStreamsResponse batchCommitWriteStreamsResponse =
            await bigQueryWriteClient.BatchCommitWriteStreamsAsync(batchCommitWriteStreamsRequest);
        if (batchCommitWriteStreamsResponse.StreamErrors?.Count > 0)
        {
            // Handle errors here.
            Console.WriteLine("Error committing write streams. Individual errors:");
            foreach (StorageError error in batchCommitWriteStreamsResponse.StreamErrors)
            {
                Console.WriteLine(error.ErrorMessage);
            }            
        }
        else
        {
            Console.WriteLine($"Writes to stream {stream.Name} have been committed.");
        }
    }
}

// [END bigquerystorage_append_rows_pending]