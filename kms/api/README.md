# .NET Cloud Key Management Service (KMS) Sample

A sample that demonstrates how to call the
[Google Cloud Key Management Service API](https://cloud.google.com/kms/docs) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://pantheon.corp.google.com/flows/enableapi?apiid=cloudkms.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Key Management Service API.

6.  Open [CloudKmsSample.sln](CloudKmsSample.sln) with Microsoft Visual Studio version 2012 or later.

7. Edit [QuickStart.cs](QuickStart/QuickStart.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```
    PS C:\...\dotnet-docs-samples\kms\api\QuickStart\bin\Debug> .\QuickStart.exe
    Key Rings:
    projects/your-project-id/locations/global/keyRings/test
    projects/your-project-id/locations/global/keyRings/testKeyRing-20170207005759502
    projects/your-project-id/locations/global/keyRings/testKeyRing-20170207005804457
    ```

10. And run CloudKmsSample.exe to see a list of subcommands like encrypt and decrypt:
    ```
    PS C:\...\dotnet-docs-samples\kms\api\bin\Debug> .\CloudKmsSample.exe
    CloudKmsSample 1.0.0.0
    Copyright c Google Inc 2016

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

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
