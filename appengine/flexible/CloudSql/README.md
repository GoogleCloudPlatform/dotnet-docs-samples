# Google Cloud SQL and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version 1.1](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.4-download.md).

2.  Visual Studio is *optional*.  Solution files can be opened with Visual
    Studio 2017 or later.  An old, unmaintained branch of samples that work
    with Visual Studio 2015 is 
    [here](https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015).

3.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

4.  [Create a second generation Google Cloud SQL instance.](
    https://cloud.google.com/sql/docs/mysql/create-instance).

5.  Create a new database in your Google Cloud SQL instance.
    
    1.  List your database instances in [Cloud Cloud Console](
        https://pantheon.corp.google.com/sql/instances/).
    
    2.  Click your Instance Id to see Instance details.

    3.  Click DATABASES.

    4.  Click **Create database**.

    2.  For **Database name**, enter `visitors`.

    3.  Click **CREATE**.

5.  Configure SSL access to the Google Cloud SQL instance.

    1.  On the Instance details page, click **SSL**.

    2.  If you're using a MySQL database, click **Allow only SSL connections.**

    3.  Click **Create a client certificate.**

    4.  Enter a name like `aspnetcert` and click **ADD**.

    5.  Click the links to download `client-key.pem`, `client-cert.pem`, 
        and `server-ca.pem`.

    6.  Click **Close**.

    7.  Click **Users**.
    
    8.  Click **Create user account**.

    9.  Enter a name like `aspnetuser` and a password.

    10. Click **Create**.

    11. Click **AUTHORIZATION**.

    12. Click **+ Add Network**.

    13. For the name, enter `all`.

    14. For the network, enter `0.0.0.0/0`.

    15. Click **Done**.

    16. Click **Save**.

    17. At a command prompt, type:

        ```bash
        openssl pkcs12 -export -in client-cert.pem -inkey client-key.pem \
            -certfile server-ca.pem -out client.pfx
        ```

        If you don't have a machine with openssl installed, use
        [Google Cloud Shell](https://cloud.google.com/shell/docs/quickstart).

    18. Move `client.pfx` into this directory.

6.  Edit [appsettings.json](appsettings.json).  Update the connection string
    with your user, password, server ip address, etc.

## Run Locally

### ![PowerShell](../.resources/powershell.png)Using PowerShell
```psm1
PS > dotnet restore
PS > dotnet run
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio
1.  In Solution Explorer, right-click the **CloudSql** project and choose **Set as StartUp Project**
2.  Press F5.

## Deploy to App Engine

### ![PowerShell](../.resources/powershell.png)Using PowerShell

```psm1
PS > dotnet restore
PS > dotnet publish
PS > gcloud beta app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio

1.  In Solution Explorer, right-click the **CloudSql** project and choose **Publish CloudSql to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.

