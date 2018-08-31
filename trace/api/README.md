# Stackdriver Trace Sample for ASP.NET Core MVC

A sample demonstrating how to create traces using the ASP.NET Core and Google Cloud Stackdriver Trace.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/trace/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

3. In [appsettings.json](appsettings.json) change the placeholder value of
"YOUR-GOOGLE-PROJECT-ID" to be your project id.

4. From a Powershell command line, run the Trace sample:
    ```ps1
    PS C:\...\dotnet-docs-samples\trace\api> dotnet run
    Now listening on: http://localhost:5000
    Application started. Press Ctrl+C to shut down.
    Application is shutting down...
    ```

5. Open the running sample web app http://localhost:5000 in a web browser 
which will display a confirmation that trace is configured along with a 
link to initiate a trace like:

  ```
  Trace has been configured for use with this application.
  See the Stackdriver Trace dashboard in the Google Cloud Console
  to view traces for your application.

    •Initiate a trace
  ```

6. Click the "Initiate a trace" link to create a sample trace.

7. Go back to Powershell and Press Ctrl+C to close the web app which will
still be running.

8. The sample trace is viewable on the
[Stackdriver Trace](https://console.cloud.google.com/traces) page in the
Cloud Platform Console.

## Deploy to Google App Engine Flexible Environment

1.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

### ![PowerShell](../../appengine/flexible/.resources/powershell.png)Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.0\publish\app.yaml
```

### ![Visual Studio](../../appengine/flexible/.resources/visual-studio.png)Using Visual Studio

1.  In Solution Explorer, right-click the **Trace** project and choose **Publish to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
