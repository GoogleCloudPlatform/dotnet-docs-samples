# Stackdriver Error Reporting Sample

A sample demonstrating how to use the `Google.Cloud.Diagnostics.AspNetCore` package
to log all of an ASP.NET web application's errors to the Error Reporting page in the Cloud Platform Console.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or later.  That means using 
[Visual Studio 2017](https://www.visualstudio.com/), [Visual Studio Code](https://code.visualstudio.com/), 
or the command line.  Visual Studio 2015 users can use 
[this older sample](https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/error-reporting/api/diagnostics).

## Links

- [Stackdriver Error Reporting](https://cloud.google.com/error-reporting/)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=clouderrorreporting.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Stackdriver Error Reporting API.

4. In [Startup.cs](./Startup.cs) change the placeholder value of "YOUR-GOOGLE-PROJECT-ID" to be your project id.

5. From a Powershell command line, run the Error Reporting sample:

    ```ps1
    PS C:\...\dotnet-docs-samples\error-reporting\api\AspNetCore> dotnet run
    Hosting environment: Production
    Content root path: C:\...\dotnet-docs-samples\error-reporting\api\AspNetCore
    Now listening on: http://localhost:5000
    Application started. Press Ctrl+C to shut down.
    ```

6. Open the running sample web app http://localhost:5000 in a web browser 
which will invoke a generic System.Exception application error.

7. Go back to Powershell and Press Ctrl+C to close the web app which will still be running.

8. The generic error that was thrown will be logged and viewable in
    the [Error Reporting](https://console.cloud.google.com/errors) page
    in the Cloud Platform Console.

    * If this is the first time using Error Reporting in your project you'll have to click 
      the options menu at the top right side of the Error Reporting page and
      select "Turn on new error notifications for the project".
    

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)
