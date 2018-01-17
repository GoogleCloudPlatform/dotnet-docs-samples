# Google Analytics and Google App Engine Flexible Environment

This sample application demonstrates how to track events with Google Analytics
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version 1.1](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.4-download.md).

2.  Visual Studio is *optional*.  Solution files can be opened with Visual
    Studio 2017 or later.  An old, unmaintained branch of samples that work
    with Visual Studio 2015 is 
    [here](https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015).

3.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

4.  [Create a Google Analytics Property and obtain the Tracking ID](
    https://support.google.com/analytics/answer/1042508?ref_topic=1009620).

4.  Edit [appsettings.json](appsettings.json).  Replace `your-google-analytics-tracking-id` with your google analytics tracking id.

## Run Locally

### ![PowerShell](../.resources/powershell.png)Using PowerShell
```psm1
PS > dotnet restore
PS > dotnet run
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio
1.  In Solution Explorer, right-click the **Analytics** project and choose **Set as StartUp Project**
2.  Press F5.

## Deploy to App Engine

### ![PowerShell](../.resources/powershell.png)Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio

1.  In Solution Explorer, right-click the **Analytics** project and choose **Publish Analytics to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

