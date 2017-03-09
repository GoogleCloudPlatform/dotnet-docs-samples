# Google Cloud SQL and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version LTS](https://www.microsoft.com/net/download/core#/lts).

2.  Visual Studio is not required, but to build and run .NET *core* applications,
    Visual Studio users need to download and install 
	[.NET Core tools](https://www.microsoft.com/net/core#windowsvs2015).

3.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

4.  [Create a second generation Google Cloud SQL instance.](
    https://cloud.google.com/sql/docs/mysql/create-instance).

5.  Create a new database in your Google Cloud SQL instance.
    
    1.  Open your [Cloud SQL's Databases](
        https://pantheon.corp.google.com/sql/instances/napoo/databases) and
        click **New database**.

    2.  For **Database name**, enter `visitors`.

    3.  Click **ADD**.

5.  Configure SSL access to the Google Cloud SQL instance.

    1.  Open your [Cloud SQL's SSL settings.](
        https://console.cloud.google.com/sql/instances/napoo/access-control/ssl)

    2.  Click **Allow only SSL connections.**

    3.  Click **Create a client certificate.**

    4.  Enter a name like `aspnetcert` and click **ADD**.

    5.  Click the links to download `client-key.pem`, `client-cert.pem`, 
        and `server-ca.pem`.

    6.  Click **Close**.

    7.  Click **Users**.
    
    8.  Click **Create user account**.

    9.  Enter a name like `aspnetuser` and a password.

    10. Click **Create**.

    11. Click **Authorization**.

    12. Click **+ Add Network**.

    13. For the name, enter `all`.

    14. For the network, enter `0.0.0.0/0`.

    15. Click **Done**.

    16. Click **Save**.

    17. At a command prompt, type:

        ```bash
        openssl pkcs12 -export -in client-cert.pem -inkey client-key.pem -certfile server-ca.pem -out client.pfx
        ```

        If you don't have a machine with openssl installed, use
        [Google Cloud Shell](https://cloud.google.com/shell/docs/quickstart).

    18. Replace the `client.pfx` file in this directory with the `client.pfx` you created.

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

