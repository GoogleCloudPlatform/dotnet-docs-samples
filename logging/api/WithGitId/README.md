# Stackdriver Logging WithGitId Sample

A sample demonstrating how to add source context git id to Stackdriver log entries using ASP.NET Core.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/logging/api/WithGitId).

## Links

- [Cloud Logging Reference Docs](https://cloud.google.com/logging/docs/)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Edit [`StackdriverLogWriter.cs`](StackdriverLogWriter.cs), and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.  
	Replace YOUR-LOG-ID with whatever log id name you like.

3. From a Powershell command line, run the WithGitId logging sample:
    ```ps1
    PS C:\...\dotnet-docs-samples\logging\api\WithGitId> dotnet run
   ```
   Which should display:

   ```ps1
    Now listening on: http://localhost:5000
    Application started. Press Ctrl+C to shut down.
    ```

4. Open the running sample web app http://localhost:5000 in a web browser 
to see the application's test web page. 

5. Click the "submit" button on the page to generate log entries.

7. Go back to Powershell and Press Ctrl+C to close the web app which will
still be running.

8. The sample log entries are viewable on the
[Logs viewer](https://console.cloud.google.com/logs) page in the
Cloud Platform Console.


## Deploy to Google App Engine Flexible Environment

1.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

### ![PowerShell](../../../appengine/flexible/.resources/powershell.png)Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.0\publish\app.yaml
```

### ![Visual Studio](../../../appengine/flexible/.resources/visual-studio.png)Using Visual Studio

1.  In Solution Explorer, right-click the **WithGitId** project and choose **Publish to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

4. By the end of deployment, you should be able to see the test web page.
   Click the button on the page to generate log entries.

5. In Visual Studio, choose `Tools | Google Cloud Tools | Show Stackdriver Logs Viewer`

6. In the logs viewer, click the resource drop-down menu which defaults to "GCE VM Instance" and select the "Global" drop-down menu item. You should be able to see the log entries just created.
	
## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)