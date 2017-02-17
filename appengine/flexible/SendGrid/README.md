# Send Grid and Google App Engine Flexible Environment

This sample application demonstrates how to send email with Send Grid
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).
    The Google Cloud SDK is required to deploy .NET applications to App Engine.

3.  (Optional) Visual Studio is not required, but
    [Google Cloud Tools for Visual Studio](
        https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running 
    Visual Studio.

4.  [Create a SendGrid account](http://sendgrid.com/partner/google).

4.  Edit [appsettings.json](appsettings.json).  Replace 
    `your-sendgrid-api-key` with your sendgrid api key.

## Run Locally

### ![PowerShell](../.resources/powershell.png)Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet run
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio

1.  In Solution Explorer, right-click the **SendGrid** project and choose **Set as StartUp Project**
2.  Press F5.

## Deploy to App Engine

### ![PowerShell](../.resources/powershell.png)Using PowerShell


```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio


1.  In Solution Explorer, right-click the **SendGrid** project and choose 
    **Publish SendGrid to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

