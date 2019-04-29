# Hello World for Google App Engine Flexible Environment

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
PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.1\publish\app.yaml
```

## Using the Sample

The sample app listens for WebSocket connections at http://localhost:5000/ws
(when running locally) or https://YOUR-PROJECT-ID.appspot.com/ws
(when deployed to App Engine). It echos every incoming WebSocket message.
You may [send a WebSocket message](https://developer.mozilla.org/en-US/docs/Web/API/WebSocket)
using the built-in WebSocket API in your browser to test the app; many
packages can help you set up a WebSocket client as well.
