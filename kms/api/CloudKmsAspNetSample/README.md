# .NET Cloud Key Management Service (KMS) ASP.NET Sample

A sample that demonstrates how to integrate
[Google Cloud Key Management Service API](https://cloud.google.com/kms/docs)
with ASP.NET MVC's appsettings to store secrets in a simple json file.

See [Keeping Secrets in ASP.NET’s appsettings.json](https://medium.com/@SurferJeff/keeping-secrets-in-asp-nets-appsettings-json-5694e533dc87) for a discussion of this code.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the dotnet core command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudkms.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Key Management Service API.

5.  Create a new encryption key.
    ```
    PS > .\New-EncryptionKey.ps1
    projects/<your-project-id>/locations/global/keyRings/webapp/cryptoKeys/appsecrets
    ```

6.  Create a file called `appsecrets.json` and fill it with this json:
    ```
    {
        "Secrets": {
            "Word": "your-secret-word"
        }
    }
    ```

7.  Encrypt `appsecrets.json`:
    ```
    PS > .\Encrypt-AppSecrets.ps1
    ```

8.  Run the app:
    ```
    PS > dotnet run
    Using launch settings from .../Properties/launchSettings.json...
    info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
        User profile is available. Using '~/.aspnet/DataProtection-Keys' as key repository; keys will not be encrypted at rest.
    Hosting environment: Development
    Content root path: /.../kms/api/CloudKmsAspNetSample
    Now listening on: https://localhost:5001
    Now listening on: http://localhost:5000
    Application started. Press Ctrl+C to shut down.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
