# Stackdriver Error Reporting Sample

A sample demonstrating how to invoke Stackdriver Error Reporting from C#.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or later.  That means using 
[Visual Studio 2017](https://www.visualstudio.com/), [Visual Studio Code](https://code.visualstudio.com/), 
or the command line.  Visual Studio 2015 users can use 
[this older sample](https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/error-reporting/api).

## Links

- [Stackdriver Error Reporting API Reference Docs](https://cloud.google.com/error-reporting/reference/rest/)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=clouderrorreporting.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Stackdriver Error Reporting API.

3.  From a Powershell command line, set the "GOOGLE_PROJECT_ID" environment variable to id of the project you created in step 1.

    ```ps1
    PS C:\...\dotnet-docs-samples\error-reporting\api\ErrorReportingSample> $env:GOOGLE_PROJECT_ID="your project id"
    ```


4.  And run the ErrorReportingSample application:
    ```ps1
    PS C:\...\dotnet-docs-samples\error-reporting\api\ErrorReportingSample> dotnet run
    ```

4.  View your error in the [Cloud Console UI](https://console.cloud.google.com/errors)
	
## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)