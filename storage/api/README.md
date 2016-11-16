# Cloud Storage Sample

A sample demonstrating how to invoke Google Cloud Storage from C#.

## Links

- [Cloud Storage Reference Docs](https://developers.google.com/api-client-library/dotnet/apis/storage/v1)

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=storage_api&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Speech API.

6.  Open [storage.sln](storage.sln) with Microsoft Visual Studio version 2012 or later.

7.  Build the Solution.

8.  Run the sample

    From the command line, set environment variables and run:

    ```sh
    C:\...\bin\Debug> set GOOGLE_PROJECT_ID=your project id displayed on the Google Developers Console.
    C:\...\bin\Debug> set GOOGLE_BUCKET=the name of the Google Cloud Storage bucket you created.
    C:\...\bin\Debug> storage
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
