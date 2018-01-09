# Stackdriver Logging Sample

A sample demonstrating how to invoke Google Cloud Stackdriver Logging from C# using ASP.NET Core.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/logging/api).

## Links

- [Cloud Logging Reference Docs](https://cloud.google.com/logging/docs/)

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

2.  Edit [`Program.cs`](LoggingSample/Program.cs), and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.

5.  From a Powershell command line, run the Logging sample to see a list of
    subcommands:

    ```ps1
    PS C:\...\dotnet-docs-samples\logging\api\LoggingSample> dotnet run
    Usage:
    LoggingSample create-log-entry log-id new-log-entry-text
    LoggingSample list-log-entries log-id
    LoggingSample create-sink sink-id log-id
    LoggingSample list-sinks
    LoggingSample update-sink sink-id log-id
    LoggingSample delete-log log-id
    LoggingSample delete-sink sink-id
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)