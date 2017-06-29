# .NET Cloud Video Intelligence Samples

A collection of samples that demonstrate how to call the
[Google Cloud Video Intelligence API](https://cloud.google.com/video-intelligence/docs/) from C#.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

4.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=videointelligence.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud Speech API.

6.  Open [Video.sln](Video.sln) with Microsoft Visual Studio version 2012 or later.

8.  Build the Solution.

9.  From the command line, run QuickStart.exe:
    ```
    PS C:\...\dotnet-docs-samples\video\api\QuickStart\bin\Debug> .\QuickStart.exe
    Animal
    Cat
    Domestic long-haired cat
    Felidae
    Felinae
    Kitten
    Maine Coon
    Nature
    Pet
    Tabby cat
    Whiskers
    ```

10. And run Analyze.exe to analyze videos:
    ```
    PS C:\...\dotnet-docs-samples\video\api\Analyze\bin\Debug> .\Analyze.exe
    Analyze 1.0.0.0
    Copyright c  2017

    ERROR(S):
    No verb selected.

    shots      Print a list shot changes.

    labels     Print a list of labels found in the video.

    faces      Print the offsets when faces appear.

    help       Display more information on a specific command.

    version    Display version information.
    ```

    ```
    PS > .\Analyze.exe shots "gs://cloudmleap/video/next/fox-snatched.mp4"
    Start Time Offset: 41729        End Time Offset: 1000984
    Start Time Offset: 1042713      End Time Offset: 6006032
    Start Time Offset: 6047761      End Time Offset: 7007016
    ...
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
