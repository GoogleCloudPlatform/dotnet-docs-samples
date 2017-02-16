# Hello World for Google App Engine Flexible Environment

This sample application is ready to deploy to Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

3.  (Optional) Visual Studio is not required, but [Google Cloud Tools for Visual Studio](https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

## Run Locally

### Using PowerShell
```psm1
PS > dotnet restore
PS > dotnet run
```

### Using Visual Studio
Press F5.

## Deploy to App Engine

### Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### Using Visual Studio

1.  In Solution Explorer, right-click the **HelloWorld** project and choose **Publish HelloWorld to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

