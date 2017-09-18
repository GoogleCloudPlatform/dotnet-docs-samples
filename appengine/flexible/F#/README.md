# F# sample for Google App Engine Flexible Environment

This sample application demonstrates a simple F# MVC application
running in Google App Engine Flexible Environment.

The application uses the [Google Cloud Vision API](https://cloud.google.com/vision/)
to check whether an image contains naughty content.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version LTS](https://www.microsoft.com/net/download/core#/lts).


## Run Locally

### ![PowerShell](../.resources/powershell.png)Using PowerShell

```psm1
PS > bower install
PS > dotnet restore
PS > dotnet run
```

## Deploy to App Engine

### ![PowerShell](../.resources/powershell.png)Using PowerShell


```psm1
PS > bower install
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp1.1\publish\app.yaml
```
