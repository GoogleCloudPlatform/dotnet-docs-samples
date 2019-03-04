# .NET Cloud Key Management Service (KMS) Sample

A sample that demonstrates how to call the
[Google Cloud Key Management Service API](https://cloud.google.com/kms/docs) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/kms/api).

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudkms.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Key Management Service API.

10. Run the CloudKmsSample to see a list of subcommands like encrypt and decrypt:
    ```
    PS C:\...\dotnet-docs-samples\kms\api\CloudKmsSample> dotnet restore
    PS C:\...\dotnet-docs-samples\kms\api\CloudKmsSample> dotnet run
    CloudKmsSample 1.0.0
    Copyright (C) 2017 CloudKmsSample

    ERROR(S):
      No verb selected.

      createKeyRing              Create a keyRing for holding CryptoKeys.

      getKeyRing                 Get a keyRing's full path details and its create time.

      createCryptoKey            Create a cryptoKey.

      getCryptoKey               Get a cryptoKey's details.

      listKeyRings               List all KeyRings associated with your project.

      listCryptoKeys             List all CryptoKeys for the specified keyRing.

      encrypt                    Encrypt the data in a text file using Cloud KMS.

      decrypt                    Decrypt the data in a text file using Cloud KMS.

      disableCryptoKeyVersion    Disable a cryptoKey version.

      enableCryptoKeyVersion     Enable a cryptoKey version.

      destroyCryptoKeyVersion    Destroy a cryptoKey version.

      restoreCryptoKeyVersion    Restore a cryptoKey version.

      getCryptoKeyVersion        Get a cryptoKey version.

      help                       Display more information on a specific command.

      version                    Display version information.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)

## Testing

* See [TESTING.md](../../../TESTING.md)
