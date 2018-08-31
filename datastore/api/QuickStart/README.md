# Google Cloud Datastore Sample

A sample demonstrating how to invoke Google Cloud Datastore from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/datastore/api).
    
## Links

- [Cloud Datastore Docs](https://cloud.google.com/datastore/docs/)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=datastore.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Datastore API.

6.  Edit `QuickStart.cs` and replace `YOUR-PROJECT-ID` with your
    Google Project id.

9.  From the command line:
    ```ps1
    PS > dotnet restore
    PS > dotnet run
    Saved sampletask1: Buy milk
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)
