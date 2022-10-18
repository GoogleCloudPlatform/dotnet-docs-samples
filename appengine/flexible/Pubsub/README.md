# Google Pubsub and Google App Engine Flexible Environment

This sample application demonstrates how to publish and receive
messages via Google Pubsub when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.5-download.md)
    or newer.

4.  Edit [appsettings.json](appsettings.json).  Replace
    `your-project-id` with your Google project id.

## ![PowerShell](../.resources/powershell.png) Using PowerShell

### Run Locally

```psm1
PS > dotnet restore
PS > dotnet run
```

### Deploy to App Engine

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.1\publish\app.yaml
```


## ![Visual Studio](../.resources/visual-studio.png) Using Visual Studio 2017

Visual Studio is *optional*.  An old, unmaintained branch of samples that work
with Visual Studio 2015 is
[here](https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015).

[Google Cloud Tools for Visual Studio](
https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

### Run Locally

Open **Pubsub.csproj**, and Press **F5**.

### Deploy to App Engine

1.  In Solution Explorer, right-click the **Pubsub** project and choose **Publish Pubsub to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.
