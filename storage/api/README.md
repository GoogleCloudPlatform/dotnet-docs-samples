# Cloud Storage Sample

A sample demonstrating how to invoke [Google Cloud Storage](
https://cloud.google.com/storage/docs/) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.  Visual Studio 2015 users
can use [this older sample](
    https://github.com/GoogleCloudPlatform/dotnet-docs-samples/tree/vs2015/storage/api).

## Links

- [Cloud Storage Client Libraries](https://cloud.google.com/storage/docs/reference/libraries#client-libraries-install-csharp)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=storage_api&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Storage API.

7.  Edit `QuickStart\Program.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.

7.  Edit `Storage\Storage.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.

8.  Build:

    ```ps1
    PS > dotnet restore
    PS > dotnet build
    ```

9.  From the command line, run QuickStart to create a bucket:

    ```ps1
    PS > dotnet run --project .\QuickStart\QuickStart.csproj
    You already own this bucket. Please select another name.
    ```

10. From the command line, run Storage to see a list of commands:

    ```ps1
    PS > dotnet run --project .\Storage\Storage.csproj
	Usage:
	  Storage create [new-bucket-name]
	  Storage create-regional-bucket location [new-bucket-name]
	  Storage list
	  Storage list bucket-name [prefix] [delimiter]
	  Storage get-metadata bucket-name object-name
	  Storage get-bucket-metadata bucket-name
	  Storage make-public bucket-name object-name
	  Storage upload [-key encryption-key] bucket-name local-file-path [object-name]
	  Storage copy source-bucket-name source-object-name dest-bucket-name dest-object-name
	  Storage move bucket-name source-object-name dest-object-name
	  Storage download [-key encryption-key] bucket-name object-name [local-file-path]
	  Storage download-byte-range bucket-name object-name range-begin range-end [local-file-path]
	  Storage generate-signed-url bucket-name object-name
	  Storage view-bucket-iam-members bucket-name
	  Storage add-bucket-iam-member bucket-name member
	  Storage remove-bucket-iam-member bucket-name role member
	  Storage add-bucket-default-kms-key bucket-name key-location key-ring key-name
	  Storage upload-with-kms-key bucket-name key-location
				      key-ring key-name local-file-path [object-name]
	  Storage print-acl bucket-name
	  Storage print-acl bucket-name object-name
	  Storage add-owner bucket-name user-email
	  Storage add-owner bucket-name object-name user-email
	  Storage add-default-owner bucket-name user-email
	  Storage remove-owner bucket-name user-email
	  Storage remove-owner bucket-name object-name user-email
	  Storage remove-default-owner bucket-name user-email
	  Storage delete bucket-name
	  Storage delete bucket-name object-name [object-name]
	  Storage enable-requester-pays bucket-name
	  Storage disable-requester-pays bucket-name
	  Storage get-requester-pays bucket-name
	  Storage generate-encryption-key
	  Storage get-bucket-default-event-based-hold bucket-name
	  Storage enable-bucket-default-event-based-hold bucket-name
	  Storage disable-bucket-default-event-based-hold bucket-name
	  Storage lock-bucket-retention-policy bucket-name
	  Storage set-bucket-retention-policy bucket-name retention-period
	  Storage remove-bucket-retention-policy bucket-name
	  Storage get-bucket-retention-policy bucket-name
	  Storage set-object-temporary-hold bucket-name object-name
	  Storage release-object-temporary-hold bucket-name object-name
	  Storage set-object-event-based-hold bucket-name object-name
	  Storage release-object-event-based-hold bucket-name object-name
	    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
