# Stackdriver Error Reporting Sample

A sample demonstrating how to use the Google Cloud Stackdriver Diagnostics ASP.NET Core NuGet package
to log all of an ASP.NET web application's errors to the Error Reporting page in the Cloud Platform Console.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/error-reporting/api/diagnostics).

## Links

- [Stackdriver Error Reporting](https://cloud.google.com/error-reporting/)

## Build and Run

1.  **Follow the instructions in the [root README](../../../README.md)**.

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=clouderrorreporting.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Stackdriver Error Reporting API.

4. In [appsettings.json](appsettings.json) change the placeholder value of "YOUR-GOOGLE-PROJECT-ID" to be your project id.

5. From a Powershell command line, run the Error Reporting sample:

    ```ps1
    PS C:\...\dotnet-docs-samples\error-reporting\api\diagnostics> dotnet run
    Hosting environment: Production
    Content root path: C:\Users\jbsimon\updateErrorReporting\dotnet-docs-samples\error-reporting\core\diagnostics
    Now listening on: http://localhost:5000
    Application started. Press Ctrl+C to shut down.
    ```

6. Open the running sample web app http://localhost:5000 in a web browser 
which will invoke a generic System.Exception application error.

7. Go back to Powershell and Press Ctrl+C to close the web app which will still be running.

8. The generic error that was thrown will be logged and viewable in
    the [Error Reporting](https://console.cloud.google.com/errors) page
    in the Cloud Platform Console.

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)