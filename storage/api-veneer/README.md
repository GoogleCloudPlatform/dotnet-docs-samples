# Cloud Storage Sample

A sample demonstrating how to invoke Google Cloud Storage from C#.

## Links

- [Cloud Storage Reference Docs](https://developers.google.com/api-client-library/dotnet/apis/storage/v1)

## Build and Run

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=storage_api&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Speech API.

6.  Open [Storage.sln](Storage.sln) with Microsoft Visual Studio version 2012 or later.

7.  Edit `QuickStart\Program.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe to see a list of 
    subcommands:

    ```sh
    C:\...\bin\Debug> QuickStart.exe
    Usage:
      QuickStart create [new-bucket-name]
      QuickStart list
      QuickStart list bucket-name [prefix] [delimiter]
      QuickStart get-metadata bucket-name object-name
      QuickStart make-public bucket-name object-name
      QuickStart upload bucket-name local-file-path [object-name]
      QuickStart copy source-bucket-name source-object-name dest-bucket-name dest-object-name
      QuickStart move bucket-name source-object-name dest-object-name
      QuickStart download bucket-name object-name [local-file-path]
      QuickStart download-byte-range bucket-name object-name range-begin range-end [local-file-path]
      QuickStart print-acl bucket-name
      QuickStart print-acl bucket-name object-name
      QuickStart add-owner bucket-name user-email
      QuickStart add-owner bucket-name object-name user-email
      QuickStart add-default-owner bucket-name user-email
      QuickStart remove-owner bucket-name user-email
      QuickStart remove-owner bucket-name object-name user-email
      QuickStart remove-default-owner bucket-name user-email
      QuickStart delete bucket-name
      QuickStart delete bucket-name object-name [object-name] 
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
