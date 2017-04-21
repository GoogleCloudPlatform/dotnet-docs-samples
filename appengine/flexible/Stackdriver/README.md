# Stackdriver and Google App Engine Flexible Environment

This sample application demonstrates Stackdriver logging, trace and error reporting
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version LTS](https://www.microsoft.com/net/download/core#/lts).

2.  Visual Studio is not required, but to build and run .NET *core* applications,
    Visual Studio users need to download and install 
	[.NET Core tools](https://www.microsoft.com/net/core#windowsvs2015).

3.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.
	
5.  Edit [appsettings.json](appsettings.json).  Replace `your-google-project-id` with your project id.
	

## Run Locally

### ![PowerShell](../.resources/powershell.png)Using PowerShell
```psm1
PS > dotnet restore
PS > dotnet run
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio
1.  In Solution Explorer, right-click the **Stackdriver** project and choose **Set as StartUp Project**
2.  Press F5.

## Deploy to App Engine

### ![PowerShell](../.resources/powershell.png)Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio

1.  In Solution Explorer, right-click the **Stackdriver** project and choose **Publish to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

