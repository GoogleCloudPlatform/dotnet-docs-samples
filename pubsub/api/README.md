# Google Cloud Pub/Sub Sample

This C# sample demonstrates how to call the
[Google Cloud Pub/Sub API](https://cloud.google.com/pubsub/docs) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=pubsub.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Stackdriver Cloud Pub/Sub API.

6.  Open [PubsubTest.sln](PubsubTest.sln) with Microsoft Visual Studio version 2012 or later.

7. Edit [Program.cs](QuickStart/Program.cs), and replace YOUR-PROJECT-ID with the id of the project you created in step 1.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```
    PS C:\...\dotnet-docs-samples\pubsub\api\QuickStart\bin\Debug> .\QuickStart.exe
    Topic projects/YOUR-PROJECT-ID/topics/my-new-topic created.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
