# Redis Cache and Google App Engine Flexible Environment

This sample application demonstrates how to store data in a Redis Cache
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.5-download.md)
    or newer.

4.  [Sign up for a Redis Labs account.](https://redislabs.com/#signup-box)

5.  Create a [Redis Database using Redis Cloud](
        https://redislabs.com/redis-howto#create-a-database-using-redis-cloud).
    Be sure to specify `GCE/us-central1` as the Cloud for the lowest latency.
    Specify a **strong password** as your Redis instance will be publicly
    accessible.  Note the Endpoint for the database.

4.  Edit [appsettings.json](appsettings.json).  Replace
    `your-redis-endpoint` with your Redis Endpoint.

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

Open **RedisCache.csproj**, and Press **F5**.

### Deploy to App Engine

1.  In Solution Explorer, right-click the **RedisCache** project and choose **Publish RedisCache to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.
