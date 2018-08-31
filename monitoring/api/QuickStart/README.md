# Google Stackdriver Monitoring Sample

This C# sample demonstrates how to call the
[Google Stackdriver Monitoring API](https://cloud.google.com/monitoring/docs) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/monitoring/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=monitoring.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Stackdriver Monitoring API.

7.  Edit [QuickStart.cs](QuickStart/QuickStart.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

9.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\monitoring\api\Quickstart> dotnet restore
    PS C:\...\dotnet-docs-samples\monitoring\api\Quickstart> dotnet run
    Done writing time series data.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
