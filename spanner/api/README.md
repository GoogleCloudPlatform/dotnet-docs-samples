# .NET Cloud Spanner Samples

A collection of samples that demonstrate how to call the
[Google Cloud Spanner API](https://cloud.google.com/spanner/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/spanner/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=spanner.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Spanner API.

5.  In the [Cloud Console](https://console.cloud.google.com/spanner/), create a Cloud Spanner
    instance and then create a database on that instance.

7.  Edit `QuickStart\Program.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. Also, update "my-instance"
    and "my-database" with the id's of the instance and database you
    created in step 3.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\spanner\api\QuickStart> dotnet run
    Hello World
    ```

10. And run the Spanner sample to see a list of subcommands:
    ```
    PS C:\...\dotnet-docs-samples\spanner\api\Spanner> dotnet run
    Spanner 1.0.0
    Copyright (C) 2018 Spanner

    ERROR(S):
      No verb selected.

      createSampleDatabase              Create a sample Cloud Spanner database along with sample tables in your project.

      createDatabase                    Create a Cloud Spanner database in your project.

      deleteSampleData                  Delete sample data from sample Cloud Spanner database table.

      insertSampleData                  Insert sample data into sample Cloud Spanner database table.

      querySampleData                   Query sample data from sample Cloud Spanner database table.

      addColumn                         Add a column to the sample Cloud Spanner database table.

      writeDataToNewColumn              Write data to a newly added column in the sample Cloud Spanner database table.

      queryNewColumn                    Query data from a newly added column in the sample Cloud Spanner database table.

      queryDataWithTransaction          Query the sample Cloud Spanner database table using a transaction.

      readWriteWithTransaction          Update data in the sample Cloud Spanner database table using a read-write transaction.

      addIndex                          Add an index to the sample Cloud Spanner database table.

      addStoringIndex                   Add a storing index to the sample Cloud Spanner database table.

      queryDataWithIndex                Query the sample Cloud Spanner database table using an index.

      queryDataWithStoringIndex         Query the sample Cloud Spanner database table using an storing index.

      readStaleData                     Read data that is ten seconds old.

      insertStructSampleData            Insert sample data that can be queried using Spanner structs.

      queryDataWithStruct               Query sample data in the sample Cloud Spanner database table using a Spanner struct.

      queryDataWithArrayOfStruct        Query sample data in the sample Cloud Spanner database table using an array of Spanner structs.

      queryDataWithStructField          Query data sample data in the sample Cloud Spanner database table using a field in a Spanner struct.

      queryDataWithNestedStructField    Query data sample data in the sample Cloud Spanner database table using a field in a nested Spanner struct.

      batchInsertRecords                Batch insert sample records into the database.

      batchReadRecords                  Batch read sample records from the database.

      addCommitTimestamp                Add a commit timestamp column to the sample Cloud Spanner database table.

      updateDataWithTimestamp           Update data with a newly added commit timestamp column in the sample Cloud Spanner database table.

      queryDataWithTimestamp            Query data with a newly added commit timestamp column in the sample Cloud Spanner database table.

      createTableWithTimestamp          Create a new table with a commit timestamp column in the sample Cloud Spanner database table.

      writeDataWithTimestamp            Write data into table with a commit timestamp column in the sample Cloud Spanner database table.

      queryNewTableWithTimestamp        Query data from table with a commit timestamp column in the sample Cloud Spanner database table.

      querySingersTable                 Query data from the sample 'Singers' Cloud Spanner database table.

      queryAlbumsTable                  Query data from the sample 'Albums' Cloud Spanner database table.

      insertUsingDml                    Insert data using a DML statement.

      updateUsingDml                    Update data using a DML statement.

      deleteUsingDml                    Delete data using a DML statement.

      updateUsingDmlWithTimestamp       Update the timestamp value of specifc records using a DML statement.

      writeAndReadUsingDml              Insert data using a DML statement and then read the inserted data.

      updateUsingDmlWithStruct          Update data using a DML statement combined with a Spanner struct.

      writeUsingDml                     Insert multiple records using a DML statement.

      writeWithTransactionUsingDml      Update data using a DML statement within a read-write transaction.

      updateUsingPartitionedDml         Update multiple records using a partitioned DML statement.

      deleteUsingPartitionedDml         Delete multiple records using a partitioned DML statement.

      listDatabaseTables                List all the user-defined tables in the database.

      deleteDatabase                    Delete a Spanner database.

      dropSampleTables                  Drops the tables created by createSampleDatabase.

      help                              Display more information on a specific command.

      version                           Display version information.
    ```

    ```
    PS > dotnet run createSampleDatabase your-project-id my-instance my-database
    Waiting for operation to complete...
    Database create operation state: Ready
    Operation status: RanToCompletion
    Created sample database my-database on instance my-instance
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
