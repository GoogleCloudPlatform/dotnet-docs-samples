# .NET Secret Manager Sample

A sample that demonstrates how to call the
[Secret Manager API](https://cloud.google.com/secret-manager) from C#.

This sample requires [.NET Core 3.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=secretmanager.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Secret Manager API.

1. Run the SecretManagerSample to see a list of subcommands:

    ```text
    PS C:\...\dotnet-docs-samples\secretmanager\api\SecretManagerSample> dotnet restore
    PS C:\...\dotnet-docs-samples\secretmanager\api\SecretManagerSample> dotnet run
    ```

    The output will look like:

    ```text
    No verb selected.

      access-version       Access the provided secret version

      add-version          Add a new version

      create               Create a secret

      destroy-version      Destroy secret version

      delete               Delete secret

      enable-version       Enable secret version

      disable-version      Disable secret version

      get-version          Get secret version

      get                  Get secret

      iam-grant-access     Grant IAM access

      iam-revoke-access    Revoke IAM access

      list-versions        List secret versions

      list                 List secrets

      update               Update secret

      help                 Display more information on a specific command.

      version              Display version information.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
