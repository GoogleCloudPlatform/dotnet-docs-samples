# Google Cloud SQL and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

3.  (Optional) Visual Studio is not required, but [Google Cloud Tools for Visual Studio](https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

4.  [Create a second generation Google Cloud SQL instance.](
    https://cloud.google.com/sql/docs/mysql/create-instance).

5.  [Configure access to the Google Cloud SQL instance.](
    https://cloud.google.com/sql/docs/mysql/instance-access-control)

6.  Edit [appsettings.json](appsettings.json).  Replace `your-cloud-sql-connection-string` with your cloud sql connection string.
    See [Connecting to Cloud SQL from External Applications](https://cloud.google.com/sql/docs/external)
	for help connecting to your Cloud SQL instance and composing the connection string.

## Run Locally

### Using PowerShell
```psm1
PS > dotnet restore
PS > dotnet run
```

### Using Visual Studio
1.  In Solution Explorer, right-click the **CloudSql** project and choose **Set as StartUp Project**
2.  Press F5.

## Deploy to App Engine

### Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### Using Visual Studio

1.  In Solution Explorer, right-click the **CloudSql** project and choose **Publish CloudSql to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

