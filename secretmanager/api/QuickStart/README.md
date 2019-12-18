# .NET Secret Manager Sample

A sample that demonstrates how to call the
[Secret Manager API](https://cloud.google.com/secret-manager/docs) from C#.

This sample requires [.NET Core 2.1](
    https://www.microsoft.com/net/core) or later.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=secretmanager.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Secret Manager API.

1.  Edit [QuickStart.cs](QuickStart/QuickStart.cs), and replace
    `YOUR-PROJECT-ID` with the id of the project you created in step 1.

1.  From a command line with `dotnet` available:

    ```text
    PS C:\...\dotnet-docs-samples\secretmanager\api\QuickStart> dotnet restore
    PS C:\...\dotnet-docs-samples\secretmanager\api\QuickStart> dotnet run
    Secrets:
    projects/your-project-number/secrets/my-secret1
    projects/your-project-number/secrets/my-secret2
    ```


## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
