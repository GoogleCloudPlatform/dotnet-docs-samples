# .NET Cloud Spanner Samples

A collection of samples that demonstrate how to call the
[Google Cloud Spanner API](https://cloud.google.com/spanner/docs/) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=spanner.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Spanner API.

6.  Open [Spanner.sln](Spanner.sln) with Microsoft Visual Studio version 2012 or later.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```
    PS C:\...\dotnet-docs-samples\spanner\api\QuickStart\bin\Debug> .\QuickStart.exe
    Hello World
    ```

10. And run Spanner.exe to see a list of subcommands::
    ```
    PS C:\...\dotnet-docs-samples\spanner\api\bin\Debug> .\Spanner.exe
    Spanner 1.0.0.0
    Copyright c  2017

    ERROR(S):
      No verb selected.

      createDatabase               Create a Spanner database in your project.

      createSampleDatabase         Create a sample Spanner database along with sample tables in your project.

      insertSampleData             Insert sample data into sample Spanner database table.

      querySampleData              Query sample data from sample Spanner database table.

      addColumn                    Add a column to the sample Spanner database table.

      writeDataToNewColumn         Write data to a newly added column in the sample Spanner database table.

      queryNewColumn               Query data from a newly added column in the sample Spanner database table.

      queryDataWithTransaction     Query the sample Spanner database table using a transaction.

      readWriteWithTransaction     Update data in the sample Spanner database table using a read-write transaction.

      addIndex                     Add an index to the sample Spanner database table.

      queryDataWithIndex           Query the sample Spanner database table using an index.

      addStoringIndex              Add a storing index to the sample Spanner database table.

      queryDataWithStoringIndex    Query the sample Spanner database table using an storing index.

      help                         Display more information on a specific command.

      version                      Display version information.
    ```

    ```
    PS > .\Spanner.exe createSampleDatabase your-project-id my-instance my-database
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
