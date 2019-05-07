# ASP.NET with Social Log In Sample

This is the companion sample for a [medium.com story](https://medium.com/@SurferJeff/adding-social-login-to-your-asp-net-core-2-1-google-cloud-platform-application-1baae89f1dc8).

This sample application demonstrates how to let users log into your application
running in Google App Engine Flexible Environment with their Facebook or Google
credentials.  

This sample requires [.NET Core 2.1](
    https://www.microsoft.com/net/core) or newer.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the dotnet core command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudkms.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Key Management Service API.

3.  Create an App Engine application.
    [Click here](https://console.cloud.google.com/appengine) to create an
    App Engine application.

4.  Run the `Set-Up.ps1` script to create keys and buckets.
    ```
    PS > .\Set-Up.ps1
    The key ring appsecrets already exists.
    The key appkey already exists.
    Adding role roles/cloudkms.cryptoKeyDecrypter to tests@appspot.gserviceaccount.com for projects/tests/locations/global/keyRings/appsecrets.
    Updated IAM policy for keyring [appsecrets].
    bindings:
    - members:
    - serviceAccount:tests@appspot.gserviceaccount.com
    role: roles/cloudkms.cryptoKeyDecrypter
    etag: BwWIAIUIOlo=
    ...
    ```

5.  Create a file called `appsecrets.json` and fill it with:
    ```json
    {
        "Secrets": {
            "Word": "wallfish"
        },
        "Authentication": {
            "Facebook": {
                "AppId": "YOUR-FACEBOOK-APP-ID",
                "AppSecret": "YOUR-FACEBOOK-APP-SECRET"
            }
        },
        "ConnectionStrings": {
            "DefaultConnection": "Server=YOUR.SQL.SERVER-IP-ADDRESS;Database=webusers;User Id=aspnet;Password=YOUR-PASSWORD"
        }    
    }
    ```

6.  Encrypt `appsecrets.json`:
    ```
    PS > .\Encrypt-AppSecrets.ps1
    ```

7.  Initialize your database:

    **[SQL Server](https://www.microsoft.com/en-us/sql-server/default.aspx) Users:**
    ```
    PS > dotnet ef database update
    ```

    **Other Database Users:**
    ```
    PS > dotnet ef migrations remove
    PS > dotnet ef migrations add origin
    PS > dotnet ef database update
    ```

8.  Run the app locally:
    ```
    PS > dotnet run
    Using launch settings from ~/gitrepos/dds2/appengine/flexible/SocialAuth/Properties/launchSettings.json...
    info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
        User profile is available. Using '~/.aspnet/DataProtection-Keys' as key repository; keys will not be encrypted at rest.
    Hosting environment: Development
    Content root path: ~/gitrepos/dds2/appengine/flexible/SocialAuth
    Now listening on: https://localhost:5001
    Now listening on: http://localhost:5000
    Application started. Press Ctrl+C to shut down.
    ```

9.  Add permissions so the App Engine user account can encrypt and decrypt.
    ```
    PS > .\Add-KmsPermissionsToAppEngine.ps1
    Waiting for async operation operations/tmo-acf.0f97d7e0-636a-4e7c-9837-a48c2b87dc8d to complete...
    Operation finished successfully. The following command can describe the Operation details:
    gcloud services operations describe operations/tmo-acf.0f97d7e0-636a-4e7c-9837-a48c2b87dc8d
    Adding role roles/cloudkms.admin to <your-project-id>@appspot.gserviceaccount.com.
    ...
    Adding role roles/cloudkms.cryptoKeyEncrypterDecrypter to <your-project-id>@appspot.gserviceaccount.com.
    ...
    ```

10. Deploy the app to App Engine:

    ```
    PS > dotnet publish
    Microsoft (R) Build Engine version 15.7.179.6572 for .NET Core
    Copyright (C) Microsoft Corporation. All rights reserved.

    Restore completed in 51.22 ms for ~/gitrepos/dds2/appengine/flexible/SocialAuth/SocialAuthMVC.csproj.
    SocialAuthMVC -> ~/gitrepos/dds2/appengine/flexible/SocialAuth/bin/Debug/netcoreapp2.1/SocialAuthMVC.dll
    SocialAuthMVC -> ~/gitrepos/dds2/appengine/flexible/SocialAuth/bin/Debug/netcoreapp2.1/SocialAuthMVC.Views.dll
    SocialAuthMVC -> ~/gitrepos/dds2/appengine/flexible/SocialAuth/bin/Debug/netcoreapp2.1/publish/
    ```

    ```
    PS > gcloud beta app deploy bin/Debug/netcoreapp2.1/publish/app.yaml
    Services to deploy:

    descriptor:      [~/gitrepos/dds2/appengine/flexible/SocialAuth/bin/Debug/netcoreapp2.1/publish/app.yaml]
    source:          [~/gitrepos/dds2/appengine/flexible/SocialAuth/bin/Debug/netcoreapp2.1/publish]
    target project:  [<your-project-id>]
    target service:  [default]
    target version:  [20180919t092928]
    target url:      [https://<your-project-id>.appspot.com]
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
