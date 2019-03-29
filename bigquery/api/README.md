# BigQuery Sample

A sample demonstrating how to invoke BigQuery from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/bigquery/api).

## Links

- [What is BigQuery?](https://cloud.google.com/bigquery/what-is-bigquery)
- [BigQuery Reference Docs](https://developers.google.com/api-client-library/dotnet/apis/bigquery/v2)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=bigquery&showconfirmation=true)
    to visit Cloud Platform Console and enable the BigQuery API.

5. Edit [SimpleApp/Program.cs](SimpleApp/Program.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

6.  From a Powershell command line, run the SimpleApp sample:

    ```
    PS C:\...\dotnet-docs-samples\bigquery\api\SimpleApp> dotnet run
    Query Results:
    ------------
    https://stackoverflow.com/questions/13530967: 44672 views
    https://stackoverflow.com/questions/22879669: 34745 views
    https://stackoverflow.com/questions/13221978: 31584 views
    https://stackoverflow.com/questions/6607552: 27736 views
    https://stackoverflow.com/questions/16609219: 26271 views
    https://stackoverflow.com/questions/35159967: 26258 views
    https://stackoverflow.com/questions/10604135: 25860 views
    https://stackoverflow.com/questions/22004216: 23496 views
    https://stackoverflow.com/questions/10644993: 22356 views
    https://stackoverflow.com/questions/11647201: 18547 views
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
