# WebSocket with SignalR for Google App Engine Flexible Environment

This sample application is ready to deploy to Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

3.  Install the [.NET Core SDK, version 2.2](https://github.com/dotnet/core/tree/master/release-notes/2.2)
    or newer.


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
PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.2\publish\app.yaml
```

## Using the Sample

[ASP.NET Core SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction)
is an open-source library that simplifies adding real-time web functionality
to apps.

This sample echoes every incoming message. You can test it out at
http://localhost:5000 (when running locally) or https://YOUR-PROJECT-ID.appspot.com
(after deployment).