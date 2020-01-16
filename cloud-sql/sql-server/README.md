# Google Cloud SQL with SQL Server and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
with a SQL Server database when running in Google App Engine Flexible Environment.

## Prerequisites

0.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**
  
1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=sqladmin.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud SQL API.

2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

3.  Install the [.NET Core SDK, version 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.5-download.md)
    or newer.

4.  [Create a Google Cloud SQL "SQL Server" instance](
    https://console.cloud.google.com/sql/choose-instance-engine).

6.  Under the instance's "USERS" tab, create a new user. Note the "User name" and "Password".

7.  Create a new database in your Google Cloud SQL instance.
    
    1.  List your database instances in [Cloud Cloud Console](
        https://console.cloud.google.com/sql/instances/).
    
    2.  Click your Instance Id to see Instance details.

    3.  Click DATABASES.

    4.  Click **Create database**.

    2.  For **Database name**, enter `votes`.

    3.  Click **CREATE**.


8.  Edit [appsettings.json](appsettings.json).  Update the connection string
    with the "User name" and "Password" you previously created on the Cloud SQL instance.

## ![PowerShell](../.resources/powershell.png) Using PowerShell

### Run Locally

1.  Download the Google Cloud SQL Proxy by following
    the instructions in the
	[Installing the Cloud SQL Proxy](https://cloud.google.com/sql/docs/mysql/sql-proxy#install)
	section of the page for installing the Cloud SQL Proxy for MySQL, which are the same for SQL Server.

2.  Run the Google Cloud SQL Proxy in Powershell using TCP (replace "myProject" and "myInstance" with the values for your project and instance):

	```psm1
    ./cloud_sql_proxy -instances=myProject:us-central1:myInstance=tcp:1433 sqlserver -u sqlserver --host 127.0.0.1
	```

3.  Edit [appsettings.json](appsettings.json).  Replace `cloudsql` with `127.0.0.1` in the connection string.

4.  With the Cloud SQL Proxy running, open another Powershell window and run the sample app:

    ```psm1
    PS > dotnet restore
    PS > dotnet run
    ```

### Deploy to App Engine Flexible

1.  Edit [app.yaml](app.yaml).  Replace `your-project-id:us-central1:instance-name`
    with your instance connection name, and leave the tcp port number set to 
    `1433` for a SQL Server instance.

2.  Run in powershell:

    ```psm1
    PS > dotnet restore
    PS > dotnet publish
    PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.1\publish\app.yaml
    ```

## ![Visual Studio](../.resources/visual-studio.png) Using Visual Studio 2017

[Google Cloud Tools for Visual Studio](
https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

### Run Locally

1.  Download and run the [Google Cloud SQL Proxy](https://cloud.google.com/sql/docs/mysql/sql-proxy).

2.  Edit [appsettings.json](appsettings.json).  Replace `cloudsql` with `127.0.0.1` in the connection string.

3.  Open **CloudSql.csproj**, and Press **F5**.

### Deploy to App Engine Flexible

0.  Edit [app.yaml](app.yaml).  Replace `your-project-id:us-central1:instance-name`
    with your instance connection name.

1.  In Solution Explorer, right-click the **CloudSql** project and choose **Publish CloudSql to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.
