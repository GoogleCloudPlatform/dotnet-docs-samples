# F# sample for Google App Engine Flexible Environment

This sample application demonstrates a simple F# MVC application
running in Google App Engine Flexible Environment.

The application uses the [Google Cloud Vision API](https://cloud.google.com/vision/)
to check whether an image contains naughty content.

## Prerequisites

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

3.  Install the [.NET Core SDK, version 1.1](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.4-download.md).

4.  Visit [fsharp.org](http://fsharp.org/) and download F# for your system.


## ![PowerShell](../.resources/powershell.png) Using PowerShell

### Run Locally


```psm1
PS > bower install
PS > dotnet restore
PS > dotnet run
```

### Deploy to App Engine

```psm1
PS > bower install
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp1.1\publish\app.yaml
```
