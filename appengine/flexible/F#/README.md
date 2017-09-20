# F# sample for Google App Engine Flexible Environment

This sample application demonstrates a simple F# MVC application
running in Google App Engine Flexible Environment.

The application uses the [Google Cloud Vision API](https://cloud.google.com/vision/)
to check whether an image contains naughty content.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
3.  Install the [.NET Core SDK](https://www.microsoft.com/net/download/core/), versions 1.1, 2.0.0 or later.

4.  Visit [fsharp.org](http://fsharp.org/) and download F# for your system.

2.  If you want to deploy the application to App Engine, 
    install the [Google Cloud SDK](https://cloud.google.com/sdk/).
    The Google Cloud SDK
    is required to deploy .NET applications to App Engine.


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
