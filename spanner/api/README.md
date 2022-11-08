# .NET Cloud Spanner Samples

A collection of samples that demonstrate how to call the
[Google Cloud Spanner API](https://cloud.google.com/spanner/docs/) from C#.

The Quickstart requires [.NET 6](
    https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later.  That means using
[Visual Studio 2022](
    https://www.visualstudio.com/), or the command line.
The samples target .NET Standard 2.1.

## Build and Run the Quickstart

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=spanner.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Spanner API.

3.  In the [Cloud Console](https://console.cloud.google.com/spanner/), create a Cloud Spanner
    instance and then create a database on that instance.

4.  Edit `QuickStart\Program.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1. Also, update "my-instance"
    and "my-database" with the id's of the instance and database you
    created in step 3.

5.  From a Powershell command line, run the QuickStart sample:
    ```
    PS C:\...\dotnet-docs-samples\spanner\api\QuickStart> dotnet run
    Hello World
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
