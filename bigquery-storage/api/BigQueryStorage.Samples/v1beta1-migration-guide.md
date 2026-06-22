# Migrating BigQuery Storage API from v1beta1 to v1: C#

This guide shows how to migrate C# code using the BigQuery Storage API from
version `v1beta1` to `v1`.

## Key Changes

*   **Namespaces**: `Google.Cloud.BigQuery.Storage.V1Beta1` is replaced by
    `Google.Cloud.BigQuery.Storage.V1`.
*   **Service Client**: `BigQueryStorageClient` is replaced by
    `BigQueryReadClient`.
*   **Table Reference**: `TableReference` object is replaced by a simple string
    representation of the table path in `ReadSession.Table`.
*   **Session Configuration**: Configuration fields (table, format, read
    options) have moved into `ReadSession` object, which is passed in
    `CreateReadSession` request.
*   **Parallelism**: `RequestedStreams` is replaced by `MaxStreamCount`.
*   **Sharding Strategy**: `ShardingStrategy` is removed. The server now
    automatically balances the streams.
*   **Read Rows Request**: `ReadPosition` is flattened. You now pass the stream
    name directly as `ReadStream` (string) and the `Offset` (long) as a
    top-level field in `ReadRowsRequest`.

## Code Comparison

### 1. Client Initialization

**v1beta1:**

```csharp
using Google.Cloud.BigQuery.Storage.V1Beta1;

BigQueryStorageClient client = BigQueryStorageClient.Create();
// use client
```

**v1:**

```csharp
using Google.Cloud.BigQuery.Storage.V1;

BigQueryReadClient client = BigQueryReadClient.Create();
// use client
```

### 2. Creating a Read Session

**v1beta1:**

```csharp
using Google.Cloud.BigQuery.Storage.V1Beta1;
using static Google.Cloud.BigQuery.Storage.V1Beta1.CreateReadSessionRequest.Types;

TableReference tableReference = new TableReference
{
    ProjectId = "bigquery-public-data",
    DatasetId = "usa_names",
    TableId = "usa_1910_current"
};

TableReadOptions readOptions = new TableReadOptions();
readOptions.SelectedFields.Add("name");
readOptions.RowRestriction = "state = 'WA'";

CreateReadSessionRequest request = new CreateReadSessionRequest
{
    Parent = "projects/read-session-project",
    TableReference = tableReference,
    ReadOptions = readOptions,
    RequestedStreams = 1,
    Format = DataFormat.Avro,
    ShardingStrategy = ShardingStrategy.Liquid
};

ReadSession session = client.CreateReadSession(request);
```

**v1:**

```csharp
using Google.Cloud.BigQuery.Storage.V1;

// Table path can be a string or a strongly-typed TableName
var tableName = new Google.Api.Gax.ResourceNames.TableName("bigquery-public-data", "usa_names", "usa_1910_current");

ReadSession.Types.TableReadOptions readOptions = new ReadSession.Types.TableReadOptions();
readOptions.SelectedFields.Add("name");
readOptions.RowRestriction = "state = 'WA'";

// ReadSession holds the session configuration
ReadSession readSession = new ReadSession
{
    Table = tableName,
    DataFormat = DataFormat.Avro, // Format renamed to DataFormat
    ReadOptions = readOptions
};

CreateReadSessionRequest request = new CreateReadSessionRequest
{
    Parent = "projects/read-session-project",
    ReadSession = readSession,
    MaxStreamCount = 1 // RequestedStreams renamed to MaxStreamCount
};

ReadSession session = client.CreateReadSession(request);
```

### 3. Reading Rows

**v1beta1:**

```csharp
using Google.Cloud.BigQuery.Storage.V1Beta1;
using System.Threading.Tasks;

var stream = session.Streams[0];

ReadRowsRequest request = new ReadRowsRequest
{
    ReadPosition = new StreamPosition
    {
        Stream = stream, // Stream object
        Offset = 0
    }
};

BigQueryStorageClient.ReadRowsStream responseStream = client.ReadRows(request);
// In older versions, you might use MoveNext on the response stream.
// Example using async foreach if supported in that version:
await foreach (ReadRowsResponse response in responseStream.GetResponseStream())
{
    // Process response.AvroRows
}
```

**v1:**

```csharp
using Google.Cloud.BigQuery.Storage.V1;
using System.Threading.Tasks;

var stream = session.Streams[0];

// Request is flattened. Pass ReadStream (string) and Offset directly.
ReadRowsRequest request = new ReadRowsRequest
{
    ReadStream = stream.Name, // Stream name string
    Offset = 0
};

BigQueryReadClient.ReadRowsStream responseStream = client.ReadRows(request);
await foreach (ReadRowsResponse response in responseStream.GetResponseStream())
{
    // Process response.AvroRows
    // Note: Prefer using response.RowCount over response.AvroRows?.RowCount (deprecated)
}
```
